import { ChangeDetectorRef, Component, Injector, TemplateRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BsModalService } from 'ngx-bootstrap/modal';
import moment from 'moment';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import {
    CheckIfAuthorized,
    CreateOrEditCustomerDto,
    CreateOrEditPawnTicketDto,
    CreateOrEditRedeemRecordDto,
    CreateOrEditRedeemRecordTicketDto,
    CustomerServiceProxy,
    PawnItem,
    PawnItemServiceProxy,
    PawnTicketServiceProxy,
    RedeemRecord,
    RedeemRecordConsolidated,
    RedeemRecordTicket,
    RedeemRecordServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AbpModalFooterComponent } from '@shared/components/modal/abp-modal-footer.component';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '@shared/components/validation/abp-validation.summary.component';
import { InputTextModule } from 'primeng/inputtext';

interface RedeemTicketRow {
    ticket: CreateOrEditPawnTicketDto;
    customer?: CreateOrEditCustomerDto;
    items: PawnItem[];
    expanded: boolean;
    itemsLoaded: boolean;
    itemsLoading: boolean;
    redeemed: boolean;
}

@Component({
    templateUrl: './redeem-records.component.html',
    animations: [appModuleAnimation()],
    standalone: true,
    styles: [`
        .redeem-ticket-value {
            max-width: 110px;
            min-width: 90px;
        }
    `],
    imports: [CommonModule, FormsModule, AbpModalFooterComponent, AbpModalHeaderComponent, AbpValidationSummaryComponent, InputTextModule],
})
export class RedeemRecordsComponent extends AppComponentBase {
    redeemRecords: RedeemRecord[] = [];
    rows: RedeemTicketRow[] = [];
    redeemRecord = new CreateOrEditRedeemRecordDto();
    authorizedName = '';
    nameLoading = false;
    ticketNumber = '';
    compareTicketNumber = '';
    ticketLoading = false;
    calculating = false;
    saving = false;
    loading = false;
    viewRecord?: RedeemRecord;
    viewRows: RedeemTicketRow[] = [];
    viewLoading = false;
    private nameRequestId = 0;
    private calculationRequestId = 0;
    @ViewChild('editor') editor!: TemplateRef<any>;
    @ViewChild('details') details!: TemplateRef<any>;

    constructor(
        injector: Injector,
        private redeemRecordService: RedeemRecordServiceProxy,
        private customerService: CustomerServiceProxy,
        private pawnItemService: PawnItemServiceProxy,
        private pawnTicketService: PawnTicketServiceProxy,
        private modal: BsModalService,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
        this.resetCreateForm();
    }

    ngOnInit(): void {
        this.loadRedeemRecords();
    }

    loadRedeemRecords(): void {
        this.loading = true;
        this.redeemRecordService.getAll().subscribe(
            (records) => {
                this.redeemRecords = (records || []).sort((left, right) => {
                    const leftDate = left.redeemedDate?.valueOf() || 0;
                    const rightDate = right.redeemedDate?.valueOf() || 0;
                    return rightDate - leftDate;
                });
                this.loading = false;
                this.cd.detectChanges();
            },
            (error) => {
                this.loading = false;
                this.notify.error(this.errorMessage(error, 'Unable to load redeem records.'));
                this.cd.detectChanges();
            }
        );
    }

    openCreate(): void {
        if (this.loading || this.saving) return;
        this.resetCreateForm();
        this.modal.show(this.editor, { class: 'modal-xl redeem-record-modal' });
    }

    openView(record: RedeemRecord): void {
        if (!record.id) return;
        this.viewRecord = record;
        this.viewRows = [];
        this.viewLoading = true;
        this.modal.show(this.details, { class: 'modal-xl redeem-record-modal' });
        this.redeemRecordService.getById(record.id).subscribe(
            (storedRecord) => {
                this.viewRecord = storedRecord || record;
                this.redeemRecordService.getAllRedeemRecordTicketWithId(record.id).subscribe(
                    (links) => this.loadViewTickets(links || [], 0),
                    (error) => {
                        this.viewLoading = false;
                        this.notify.error(this.errorMessage(error, 'Unable to load redeem record tickets.'));
                        this.cd.detectChanges();
                    }
                );
            },
            (error) => {
                this.viewLoading = false;
                this.notify.error(this.errorMessage(error, 'Unable to load the redeem record.'));
                this.cd.detectChanges();
            }
        );
    }

    private loadViewTickets(links: RedeemRecordTicket[], index: number): void {
        if (index >= links.length) {
            this.viewLoading = false;
            this.cd.detectChanges();
            return;
        }

        const ticketId = links[index].pawnTicket;
        if (!ticketId) {
            this.loadViewTickets(links, index + 1);
            return;
        }

        this.pawnTicketService.getViaIdForEdit(ticketId).subscribe(
            (result) => {
                if (result?.pawnTicket) {
                    const row: RedeemTicketRow = {
                        ticket: result.pawnTicket,
                        items: [],
                        expanded: false,
                        itemsLoaded: false,
                        itemsLoading: false,
                        redeemed: true,
                    };
                    this.viewRows = [...this.viewRows, row];
                    this.loadCustomer(row);
                }
                this.loadViewTickets(links, index + 1);
            },
            (error) => {
                this.notify.error(this.errorMessage(error, 'Unable to load a redeemed pawn ticket.'));
                this.loadViewTickets(links, index + 1);
            }
        );
    }

    private resetCreateForm(): void {
        this.rows = [];
        this.authorizedName = '';
        this.nameLoading = false;
        this.ticketNumber = '';
        this.compareTicketNumber = '';
        this.ticketLoading = false;
        this.calculating = false;
        this.redeemRecord = new CreateOrEditRedeemRecordDto();
        this.redeemRecord.redeemedDate = moment();
        this.redeemRecord.numberOfTickets = 0;
        this.redeemRecord.totalAmount = 0;
        this.redeemRecord.totalCustody = 0;
        this.redeemRecord.totalService = 0;
        this.redeemRecord.grandTotal = 0;
        this.redeemRecord.totalDiscount = 0;
    }

    onRedeemedByChange(value: string): void {
        const redeemedBy = (value || '').trim();
        this.redeemRecord.redeemedBy = value;
        this.authorizedName = '';
        this.rows = [];
        this.ticketNumber = '';
        this.compareTicketNumber = '';
        const requestId = ++this.nameRequestId;
        this.calculationRequestId += 1;
        this.calculating = false;

        if (!redeemedBy) {
            this.nameLoading = false;
            this.resetTotals();
            return;
        }

        this.nameLoading = true;
        this.redeemRecordService.getNameOfTheAuthorized(redeemedBy, undefined).subscribe(
            (name) => {
                if (requestId !== this.nameRequestId || this.redeemRecord.redeemedBy?.trim() !== redeemedBy) return;
                this.authorizedName = (name || '').trim();
                this.nameLoading = false;
                this.resetTotals();
                this.cd.detectChanges();
            },
            (error) => {
                if (requestId !== this.nameRequestId) return;
                this.nameLoading = false;
                this.notify.error(this.errorMessage(error, 'Unable to verify the redeemed-by NRIC.'));
                this.cd.detectChanges();
            }
        );
    }

    verifyTicket(): void {
        const redeemedBy = (this.redeemRecord.redeemedBy || '').trim();
        const ticketNumber = this.ticketNumber.trim();
        const compareTicketNumber = this.compareTicketNumber.trim();
        if (!this.authorizedName || !redeemedBy || !ticketNumber || ticketNumber !== compareTicketNumber) {
            this.notify.warn('Enter the same pawn ticket number in both fields.');
            return;
        }
        if (this.rows.some((row) => row.ticket.ticketNo === ticketNumber)) {
            this.notify.warn('This pawn ticket has already been added.');
            return;
        }

        const input = new CheckIfAuthorized({ nric: redeemedBy, ticketNumber });
        this.ticketLoading = true;
        this.redeemRecordService.checkIfPawnTicketValidByAuthorizedPeople(input).subscribe(
            (valid) => {
                if (!valid) {
                    this.ticketLoading = false;
                    this.notify.error('This pawn ticket is not linked to the redeemed-by customer or an authorized person.');
                    this.cd.detectChanges();
                    return;
                }
                this.redeemRecordService.getTheRelatedPawnTicketViaTicketNumber(redeemedBy, ticketNumber).subscribe(
                    (ticket) => this.finishTicketLookup(input, ticket),
                    (error) => {
                        this.ticketLoading = false;
                        this.notify.error(this.errorMessage(error, 'Unable to load the pawn ticket.'));
                        this.cd.detectChanges();
                    }
                );
            },
            (error) => {
                this.ticketLoading = false;
                this.notify.error(this.errorMessage(error, 'Unable to validate the pawn ticket.'));
                this.cd.detectChanges();
            }
        );
    }

    private finishTicketLookup(input: CheckIfAuthorized, ticket: CreateOrEditPawnTicketDto): void {
        if (!ticket?.id) {
            this.ticketLoading = false;
            this.notify.error('The pawn ticket could not be found.');
            this.cd.detectChanges();
            return;
        }

        this.redeemRecordService.checkIfPawnTicketRedeemed(input).subscribe(
            (redeemed) => {
                const row: RedeemTicketRow = {
                    ticket,
                    items: [],
                    expanded: false,
                    itemsLoaded: false,
                    itemsLoading: false,
                    redeemed: !!redeemed,
                };
                this.rows = [...this.rows, row];
                this.ticketNumber = '';
                this.compareTicketNumber = '';
                this.ticketLoading = false;
                this.loadCustomer(row);
                if (!row.redeemed) {
                    this.recalculate();
                } else {
                    this.notify.warn('This pawn ticket has already been redeemed and will not be included in this redeem record.');
                }
                this.cd.detectChanges();
            },
            (error) => {
                this.ticketLoading = false;
                this.notify.error(this.errorMessage(error, 'Unable to check whether the pawn ticket was redeemed.'));
                this.cd.detectChanges();
            }
        );
    }

    private loadCustomer(row: RedeemTicketRow): void {
        if (!row.ticket.customer) return;
        this.customerService.getViaIdForEdit(row.ticket.customer).subscribe(
            (result) => {
                row.customer = result?.customer;
                this.cd.detectChanges();
            },
            (error) => this.notify.error(this.errorMessage(error, 'Unable to load the pawn ticket customer.'))
        );
    }

    toggleItems(row: RedeemTicketRow): void {
        row.expanded = !row.expanded;
        if (!row.expanded || row.itemsLoaded || !row.ticket.id) return;
        row.itemsLoading = true;
        this.pawnItemService.getAllViaPawnTicketId(row.ticket.id).subscribe(
            (items) => {
                row.items = items || [];
                row.itemsLoaded = true;
                row.itemsLoading = false;
                this.cd.detectChanges();
            },
            (error) => {
                row.itemsLoading = false;
                this.notify.error(this.errorMessage(error, 'Unable to load pawn items.'));
                this.cd.detectChanges();
            }
        );
    }

    removeTicket(row: RedeemTicketRow): void {
        this.rows = this.rows.filter((candidate) => candidate !== row);
        this.recalculate();
        this.cd.detectChanges();
    }

    onTicketValueChange(): void {
        this.recalculate();
    }

    private recalculate(): void {
        const requestId = ++this.calculationRequestId;
        const activeRows = this.rows.filter((row) => !row.redeemed && !!row.ticket.id);
        if (!activeRows.length) {
            this.calculating = false;
            this.resetTotals();
            return;
        }

        this.calculating = true;
        const consolidated = new RedeemRecordConsolidated({
            createOrEditRedeemRecordDto: this.redeemRecord,
            createOrEditRedeemRecordTicketDtos: [],
            createOrEditPawnTicketDtos: activeRows.map((row) => row.ticket),
        });
        this.redeemRecordService.dynamicCalculateGrandTotal(consolidated).subscribe(
            (result) => {
                if (requestId !== this.calculationRequestId) return;
                this.redeemRecord = result?.createOrEditRedeemRecordDto || this.redeemRecord;
                (result?.createOrEditPawnTicketDtos || []).forEach((ticket) => {
                    const row = this.rows.find((candidate) => candidate.ticket.id === ticket.id);
                    if (row) row.ticket = ticket;
                });
                this.calculating = false;
                this.cd.detectChanges();
            },
            (error) => {
                if (requestId !== this.calculationRequestId) return;
                this.calculating = false;
                this.notify.error(this.errorMessage(error, 'Unable to calculate the redeem total.'));
                this.cd.detectChanges();
            }
        );
    }

    save(): void {
        const activeRows = this.rows.filter((row) => !row.redeemed && !!row.ticket.id);
        if (!this.authorizedName || !activeRows.length || this.calculating || this.ticketLoading) {
            this.notify.warn('Verify at least one eligible pawn ticket before saving.');
            return;
        }

        this.redeemRecord.redeemedBy = this.redeemRecord.redeemedBy?.trim();
        this.redeemRecord.redeemedDate = this.redeemRecord.redeemedDate || moment();
        this.saving = true;
        this.redeemRecordService.createOrEdit(this.redeemRecord).subscribe(
            (result) => {
                if (!result?.id) {
                    this.saving = false;
                    this.notify.error(result?.message || 'The redeem record was not created.');
                    this.cd.detectChanges();
                    return;
                }
                this.createTicketLinks(result.id, activeRows, 0, result.message);
            },
            (error) => {
                this.saving = false;
                this.notify.error(this.errorMessage(error, 'Unable to save the redeem record.'));
                this.cd.detectChanges();
            }
        );
    }

    private createTicketLinks(recordId: number, rows: RedeemTicketRow[], index: number, message?: string): void {
        if (index >= rows.length) {
            this.saving = false;
            this.notify.info(message || this.l('SavedSuccessfully'));
            this.modal.hide();
            this.loadRedeemRecords();
            return;
        }

        const ticketId = rows[index].ticket.id;
        this.redeemRecordService.createRedeemRecordTicket(new CreateOrEditRedeemRecordTicketDto({ id: undefined, pawnTicket: ticketId, redeemRecord: recordId })).subscribe(
            () => this.createTicketLinks(recordId, rows, index + 1, message),
            (error) => {
                this.saving = false;
                this.notify.error(this.errorMessage(error, 'The redeem record was saved, but a pawn ticket link could not be created.'));
                this.cd.detectChanges();
            }
        );
    }

    deleteRecord(record: RedeemRecord): void {
        if (!record.id || !confirm(`Delete redeem record ${record.id}?`)) return;
        this.redeemRecordService.delete(record.id).subscribe(
            () => {
                this.notify.info(this.l('SuccessfullyDeleted'));
                this.loadRedeemRecords();
            },
            (error) => this.notify.error(this.errorMessage(error, 'Unable to delete the redeem record.'))
        );
    }

    itemListingName(row: PawnItem): string {
        return row.itemListingFk?.description || row.description || '-';
    }

    hasEligibleRows(): boolean {
        return this.rows.some((row) => !row.redeemed);
    }

    private resetTotals(): void {
        this.redeemRecord.numberOfTickets = 0;
        this.redeemRecord.totalAmount = 0;
        this.redeemRecord.totalCustody = 0;
        this.redeemRecord.totalService = 0;
        this.redeemRecord.grandTotal = 0;
        this.redeemRecord.totalDiscount = 0;
    }

    private errorMessage(error: any, fallback: string): string {
        return error?.error?.error?.message || error?.error?.message || error?.message || fallback;
    }
}
