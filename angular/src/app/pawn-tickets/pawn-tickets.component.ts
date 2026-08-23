import { ChangeDetectorRef, Component, Injector, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { PawnTicketDto } from '@shared/service-proxies/service-proxies';
import { LookupServiceProxy } from '@shared/service-proxies/lookup-service-proxy';
import { CreatePawnTicketDialogComponent } from './create-pawn-ticket/create-pawn-ticket-dialog.component';
import { EditPawnTicketDialogComponent } from './edit-pawn-ticket/edit-pawn-ticket-dialog.component';
import { Table, TableModule } from 'primeng/table';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { ActivatedRoute, NavigationStart, Router } from '@angular/router';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { FormsModule } from '@angular/forms';
import { DatePipe, NgIf } from '@angular/common';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { ButtonModule } from 'primeng/button';

@Component({
    templateUrl: './pawn-tickets.component.html',
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [FormsModule, TableModule, PrimeTemplate, NgIf, DatePipe, PaginatorModule, LocalizePipe, ButtonModule],
})
export class PawnTicketsComponent extends PagedListingComponentBase<PawnTicketDto> {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    keyword = '';

    constructor(
        injector: Injector,
        private _lookupService: LookupServiceProxy,
        private _modalService: BsModalService,
        private _activatedRoute: ActivatedRoute,
        private _router: Router,
        cd: ChangeDetectorRef
    ) {
        super(injector, cd);
        this.keyword = this._activatedRoute.snapshot.queryParams['keyword'] || '';
        this._router.events.subscribe((event) => {
            if (event instanceof NavigationStart) {
                this._modalService.hide();
            }
        });
    }

    list(event?: LazyLoadEvent): void {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);

            if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
                return;
            }
        }

        this.primengTableHelper.showLoadingIndicator();

        this._lookupService
            .getPawnTickets(
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
            .subscribe((result) => {
                this.primengTableHelper.records = (result && result.items) || [];
                this.primengTableHelper.totalRecordsCount = (result && result.totalCount) || 0;
                this.primengTableHelper.hideLoadingIndicator();
                this.cd.detectChanges();
            });
    }

    delete(ticket: PawnTicketDto): void {
        abp.message.confirm(this.l('DeletePawnTicketWarningMessage', ticket.ticketNo), undefined, (result: boolean) => {
            if (result) {
                this._lookupService
                    .deletePawnTicket(ticket.id)
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

    createPawnTicket(): void {
        this.showCreateOrEditPawnTicketDialog();
    }

    editPawnTicket(ticket: PawnTicketDto): void {
        this.showCreateOrEditPawnTicketDialog(ticket.id);
    }

    showCreateOrEditPawnTicketDialog(id?: number): void {
        if (this._modalService.getModalsCount() > 0) {
            return;
        }

        let dialog: BsModalRef;
        if (!id) {
            dialog = this._modalService.show(CreatePawnTicketDialogComponent, {
                class: 'modal-xl',
            });
        } else {
            dialog = this._modalService.show(EditPawnTicketDialogComponent, {
                class: 'modal-xl',
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
