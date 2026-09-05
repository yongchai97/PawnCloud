import { ChangeDetectorRef, Component, Injector, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { BasicCodeServiceProxy, BasicCodeDto, BasicCodeDtoPagedResultDto } from '@shared/service-proxies/service-proxies';
import { CreateBasicCodeDialogComponent } from './create-basic-code/create-basic-code-dialog.component';
import { EditBasicCodeDialogComponent } from './edit-basic-code/edit-basic-code-dialog.component';
import { Table, TableModule } from 'primeng/table';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { ButtonModule } from 'primeng/button';

@Component({
    templateUrl: './basic-codes.component.html',
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [FormsModule, TableModule, PrimeTemplate, NgIf, PaginatorModule, LocalizePipe, ButtonModule],
})
export class BasicCodesComponent extends PagedListingComponentBase<BasicCodeDto> {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    keyword = '';

    constructor(
        injector: Injector,
        private _basicCodeService: BasicCodeServiceProxy,
        private _modalService: BsModalService,
        cd: ChangeDetectorRef
    ) {
        super(injector, cd);
    }

    list(event?: LazyLoadEvent): void {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);

            if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
                return;
            }
        }

        this.primengTableHelper.showLoadingIndicator();

        this._basicCodeService
            .getAll(
                this.keyword,
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event)
            )
            .pipe(
                finalize(() => {
                    this.primengTableHelper.hideLoadingIndicator();
                })
            )
            .subscribe((result: BasicCodeDtoPagedResultDto) => {
                this.primengTableHelper.records = result.items || [];
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.hideLoadingIndicator();
                this.cd.detectChanges();
            });
    }

    delete(basicCode: BasicCodeDto): void {
        abp.message.confirm(this.l('DeleteWarningMessage', basicCode.codeName), undefined, (result: boolean) => {
            if (result) {
                this._basicCodeService
                    .delete(basicCode.id)
                    .subscribe(() => {
                        abp.notify.success(this.l('SuccessfullyDeleted'));
                        this.refresh();
                    });
            }
        });
    }

    createBasicCode(): void {
        this.showCreateOrEditBasicCodeDialog();
    }

    editBasicCode(basicCode: BasicCodeDto): void {
        if (basicCode.systemProvidedValue) return;
        this.showCreateOrEditBasicCodeDialog(basicCode.id);
    }

    showCreateOrEditBasicCodeDialog(id?: number): void {
        let dialog: BsModalRef;
        if (!id) {
            dialog = this._modalService.show(CreateBasicCodeDialogComponent, {
                class: 'modal-lg',
            });
        } else {
            dialog = this._modalService.show(EditBasicCodeDialogComponent, {
                class: 'modal-lg',
                initialState: {
                    id: id,
                },
            });
        }

        dialog.content.onSave.subscribe(() => {
            this.refresh();
        });
    }
}
