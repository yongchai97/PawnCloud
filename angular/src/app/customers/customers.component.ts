import { ChangeDetectorRef, Component, Injector, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { CustomerServiceProxy, CustomerDto, CustomerDtoPagedResultDto } from '@shared/service-proxies/service-proxies';
import { CreateCustomerDialogComponent } from './create-customer/create-customer-dialog.component';
import { EditCustomerDialogComponent } from './edit-customer/edit-customer-dialog.component';
import { Table, TableModule } from 'primeng/table';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { ActivatedRoute } from '@angular/router';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { ButtonModule } from 'primeng/button';

@Component({
    templateUrl: './customers.component.html',
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [FormsModule, TableModule, PrimeTemplate, NgIf, PaginatorModule, LocalizePipe, ButtonModule],
})
export class CustomersComponent extends PagedListingComponentBase<CustomerDto> {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    keyword = '';

    constructor(
        injector: Injector,
        private _customerService: CustomerServiceProxy,
        private _modalService: BsModalService,
        private _activatedRoute: ActivatedRoute,
        cd: ChangeDetectorRef
    ) {
        super(injector, cd);
        this.keyword = this._activatedRoute.snapshot.queryParams['keyword'] || '';
    }

    list(event?: LazyLoadEvent): void {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);

            if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
                return;
            }
        }

        this.primengTableHelper.showLoadingIndicator();

        this._customerService
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
            .subscribe((result: CustomerDtoPagedResultDto) => {
                this.primengTableHelper.records = result.items || [];
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.hideLoadingIndicator();
                this.cd.detectChanges();
            });
    }

    delete(customer: CustomerDto): void {
        abp.message.confirm(this.l('DeleteCustomerWarningMessage', customer.fullName), undefined, (result: boolean) => {
            if (result) {
                this._customerService
                    .delete(customer.id)
                    .pipe(
                        finalize(() => {
                            abp.notify.success(this.l('SuccessfullyDeleted'));
                            this.refresh();
                        })
                    )
                    .subscribe(() => {});
            }
        });
    }

    createCustomer(): void {
        this.showCreateOrEditCustomerDialog();
    }

    editCustomer(customer: CustomerDto): void {
        this.showCreateOrEditCustomerDialog(customer.id);
    }

    showCreateOrEditCustomerDialog(id?: number): void {
        let dialog: BsModalRef;
        if (!id) {
            dialog = this._modalService.show(CreateCustomerDialogComponent, {
                class: 'modal-lg',
            });
        } else {
            dialog = this._modalService.show(EditCustomerDialogComponent, {
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


