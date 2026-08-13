import { ChangeDetectorRef, Component, Injector, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { GoldTypeServiceProxy, GoldTypeDto, GoldTypeDtoPagedResultDto } from '@shared/service-proxies/service-proxies';
import { Table, TableModule } from 'primeng/table';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { ButtonModule } from 'primeng/button';

@Component({
    templateUrl: './gold-types.component.html',
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [FormsModule, TableModule, PrimeTemplate, NgIf, PaginatorModule, LocalizePipe, ButtonModule],
})
export class GoldTypesComponent extends PagedListingComponentBase<GoldTypeDto> {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    keyword = '';

    constructor(
        injector: Injector,
        private _goldTypeService: GoldTypeServiceProxy,
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

        this._goldTypeService
            .getAll(
                this.keyword,
                this.primengTableHelper.getSorting(this.dataTable),
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event)
            )
            .pipe(
                finalize(() => {
                    this.primengTableHelper.hideLoadingIndicator();
                })
            )
            .subscribe((result: GoldTypeDtoPagedResultDto) => {
                this.primengTableHelper.records = result.items || [];
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.hideLoadingIndicator();
                this.cd.detectChanges();
            });
    }

    delete(entity: GoldTypeDto): void {
        abp.message.confirm(this.l('DeleteConfirmation', entity.purity), undefined, (result: boolean) => {
            if (result) {
                // Add delete functionality if needed
                // this._goldTypeService.delete(entity.id).subscribe(() => {
                //     abp.notify.success(this.l('SuccessfullyDeleted'));
                //     this.refresh();
                // });
            }
        });
    }
}
