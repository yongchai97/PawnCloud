import { Component, ChangeDetectorRef, Injector, TemplateRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { LookupServiceProxy, CustomerLookupDto, PawnTicketLookupDto } from '@shared/service-proxies/lookup-service-proxy';
import { OutletServiceProxy } from '@shared/service-proxies/outlet-service-proxy';
import {
    BasicCodeLookupDto, Country, CountryServiceProxy, CreateOrEditCustomerDto, CreateOrEditPawnItemDto, CreateOrEditPawnTicketDto,
    FileParameter, GeneralSetup, PawnTicketDocument, PawnTicketDocumentServiceProxy, PawnTicketPayment, PawnTicketPaymentDto,
    CustomerServiceProxy,
    CreatePawnTicketWithItemsDto,
    GoldType, GoldTypeServiceProxy, ItemListing, ItemListingServiceProxy, ItemStatus,
    ItemStatusServiceProxy, PawnItemServiceProxy, PawnTicketDto, PawnTicketPaymentServiceProxy, PawnTicketServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { DropdownModule } from 'primeng/dropdown';

@Component({
    templateUrl: './pawn-tickets.component.html',
    animations: [appModuleAnimation()],
    standalone: true,
    imports: [CommonModule, FormsModule, LocalizePipe, ButtonModule, InputTextModule, InputNumberModule, DropdownModule],
})
export class PawnTicketsComponent extends AppComponentBase {
    tickets: PawnTicketDto[] = [];

    outlets: GeneralSetup[] = [];

    documents: PawnTicketDocument[] = [];

    documentUrls = new Map<number, string>();

    uploadingDocument = false;
    payments: PawnTicketPayment[] = [];
    paymentMethods: { id: number; label: string }[] = [];
    newPaymentAmount = 0;
    newPaymentMethod?: number;
    customers: CustomerLookupDto[] = [];
    selectedCustomer?: CreateOrEditCustomerDto;
    countryNames = new Map<number, string>();
    ticketOptions: PawnTicketLookupDto[] = [];
    itemListings: ItemListing[] = [];
    itemStatuses: ItemStatus[] = [];
    goldTypes: GoldType[] = [];
    itemListingOptions: { id: number; label: string }[] = [];
    itemStatusOptions: { id: number; label: string }[] = [];
    goldTypeOptions: { id: number; label: string }[] = [];
    includedItems: BasicCodeLookupDto[] = [];
    customerFilter = '';
    selectedTicketId?: number;
    ticket = new CreateOrEditPawnTicketDto();
    items: CreateOrEditPawnItemDto[] = [];
    saving = false;
    calculating = false;
    pageLoading = true;
    private pendingInitialLoads = 9;
    @ViewChild('editor') editor!: TemplateRef<any>;

    constructor(
        injector: Injector,
        private lookup: LookupServiceProxy,
        private customerService: CustomerServiceProxy,
        private countryService: CountryServiceProxy,
        private ticketService: PawnTicketServiceProxy,
        private listingService: ItemListingServiceProxy,
        private statusService: ItemStatusServiceProxy,
        private goldService: GoldTypeServiceProxy,
        private pawnItemService: PawnItemServiceProxy,
        private outletService: OutletServiceProxy,
        private documentService: PawnTicketDocumentServiceProxy,
        private paymentService: PawnTicketPaymentServiceProxy,
        private modal: BsModalService,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
        this.resetForm();
    }

    ngOnInit(): void {
        this.loadTickets();

        this.outletService.getAll().pipe(finalize(() => this.completeInitialLoad())).subscribe((outlets) => {
            this.outlets = (outlets || []).filter((outlet) => !!outlet.id && !!outlet.outletName);
            this.cd.detectChanges();
        });
        this.loadCustomers();
        this.countryService.getAllViaYear(new Date().getFullYear()).pipe(finalize(() => this.completeInitialLoad())).subscribe((countries: Country[]) => {
            (countries || []).forEach((country) => this.countryNames.set(country.id, country.countryName));
            this.cd.detectChanges();
        });
        this.listingService.getAll().pipe(finalize(() => this.completeInitialLoad())).subscribe((items) => {
            this.itemListings = items || [];
            this.itemListingOptions = this.itemListings.map((item) => ({ id: item.id, label: [item.code, item.description].filter(Boolean).join(' - ') }));
            this.items.forEach((item) => this.updateItemDescription(item));
        });
        this.statusService.getAll().pipe(finalize(() => this.completeInitialLoad())).subscribe((items) => {
            this.itemStatuses = items || [];
            this.itemStatusOptions = this.itemStatuses.map((item) => ({ id: item.id, label: [item.code, item.description].filter(Boolean).join(' - ') }));
            this.items.forEach((item) => this.updateItemDescription(item));
        });
        this.goldService.getAll().pipe(finalize(() => this.completeInitialLoad())).subscribe((items) => {
            this.goldTypes = items || [];
            this.goldTypeOptions = this.goldTypes.map((item) => ({ id: item.id, label: [item.purity, item.description].filter(Boolean).join(' - ') }));
        });
        this.lookup.getMiscMasterConfigs().subscribe((result) => {
            const includedItemCategory = (result.items || []).find((category) => category.displayName?.toUpperCase() === 'INCLUDED ITEM');
            if (includedItemCategory) {
                this.lookup.getBasicCodesByCategory(includedItemCategory.id).pipe(finalize(() => this.completeInitialLoad())).subscribe((codes) => {
                    this.includedItems = codes.items || [];
                    this.cd.detectChanges();
                });
            } else {
                this.completeInitialLoad();
            }
            const paymentCategory = (result.items || []).find((category) => category.displayName?.toUpperCase().includes('PAYMENT METHOD'));
            if (paymentCategory) {
                this.lookup.getBasicCodesByCategory(paymentCategory.id).subscribe((codes) => {
                    this.paymentMethods = (codes.items || []).map((code) => ({ id: code.id, label: code.displayName || '' }));
                    this.cd.detectChanges();
                });
            }
        });
    }

    private completeInitialLoad(): void {
        if (!this.pageLoading) return;
        this.pendingInitialLoads -= 1;
        if (this.pendingInitialLoads <= 0) {
            this.pageLoading = false;
            this.cd.detectChanges();
        }
    }

    loadTickets(): void {
        this.lookup.getPawnTicketsForLookup().pipe(finalize(() => this.completeInitialLoad())).subscribe((result) => (this.ticketOptions = result.items || []));
        this.ticketService.getAll('', 'CreationTime DESC', 0, 1000).pipe(finalize(() => this.completeInitialLoad())).subscribe((result) => (this.tickets = result.items || []));
    }

    loadCustomers(): void {
        this.lookup.getCustomersForLookup(this.customerFilter).pipe(finalize(() => this.completeInitialLoad())).subscribe((result) => (this.customers = result.items || []));
    }

    selectCustomer(): void {
        if (!this.ticket.customer) {
            this.selectedCustomer = undefined;
            return;
        }
        this.customerService.getViaIdForEdit(this.ticket.customer).subscribe((result) => {
            this.selectedCustomer = result.customer;
            this.cd.detectChanges();
        });
    }

    customerIdentity(): string {
        return this.selectedCustomer?.nric || this.selectedCustomer?.passportNo || '';
    }

    countryName(id: number | undefined): string {
        return id ? this.countryNames.get(id) || '' : '';
    }

    updateItemDescription(item: CreateOrEditPawnItemDto): void {
        const listing = this.itemListings.find((option) => option.id === item.itemListing);
        const status = this.itemStatuses.find((option) => option.id === item.itemStatus);
        item.description = [listing?.description, status?.description].filter(Boolean).join(' ');
        this.cd.detectChanges();
    }

    openCreate(): void {
        this.resetForm();
        this.modal.show(this.editor, { class: 'modal-xl pawn-ticket-modal' });
    }

    openEdit(ticket: PawnTicketDto): void {
        this.resetForm();
        this.ticket.id = ticket.id;
        this.ticketService.getViaIdForEdit(ticket.id).subscribe((result) => {
            this.ticket = result.pawnTicket || this.ticket;
            this.selectCustomer();
            this.pawnItemService.getAllViaPawnTicketId(ticket.id).subscribe((items) => {
                this.items = (items || []).map((item) => new CreateOrEditPawnItemDto({ ...item, id: undefined }));
                if (this.items.length === 0) this.addItem();
                this.loadDocuments();
                this.loadPayments();
                this.modal.show(this.editor, { class: 'modal-xl pawn-ticket-modal' });
                this.cd.detectChanges();
            });
        });
    }

    loadDocuments(): void {
        if (!this.ticket.id) {
            this.documents = [];
            return;
        }
        this.documentService.getList(this.ticket.id).subscribe((documents) => {
            this.documents = documents || [];
            this.documents.forEach((document) => this.loadDocumentUrl(document));
            this.cd.detectChanges();
        });
    }

    loadPayments(): void {
        if (!this.ticket.id) {
            this.payments = [];
            return;
        }
        this.paymentService.getByPawnTicketId(this.ticket.id).subscribe((payments) => {
            this.payments = payments || [];
            this.cd.detectChanges();
        });
    }

    paymentTotal(): number {
        return this.payments.reduce((total, payment) => total + (Number(payment.amount) || 0), 0);
    }

    paymentRemaining(): number {
        return Math.max(0, (Number(this.ticket.amount) || 0) - this.paymentTotal());
    }

    addPayment(): void {
        const amount = Number(this.newPaymentAmount) || 0;
        if (!this.ticket.id || amount <= 0) {
            this.notify.warn('Enter a payment amount greater than zero.');
            return;
        }
        if (amount > this.paymentRemaining()) {
            this.notify.error('Payment total cannot exceed the pawn ticket amount.');
            return;
        }
        const payment = new PawnTicketPaymentDto({ id: undefined, pawnTicket: this.ticket.id, amount, paymentMethod: this.newPaymentMethod });
        this.paymentService.create(payment).subscribe(() => {
            this.newPaymentAmount = 0;
            this.newPaymentMethod = undefined;
            this.notify.info('Pawn ticket payment added.');
            this.loadPayments();
        });
    }

    deletePayment(payment: PawnTicketPayment): void {
        if (!payment.id || !confirm('Delete this pawn ticket payment?')) return;
        this.paymentService.delete(payment.id).subscribe(() => {
            this.notify.info('Pawn ticket payment deleted.');
            this.loadPayments();
        });
    }

    paymentMethodName(id: number | undefined): string {
        return this.paymentMethods.find((method) => method.id === id)?.label || '';
    }

    uploadDocument(event: Event): void {
        const file = (event.target as HTMLInputElement).files?.[0];
        if (!file || !this.ticket.id) return;
        this.uploadingDocument = true;
        const fileParameter: FileParameter = { data: file, fileName: file.name };
        this.documentService.upload(this.ticket.id, fileParameter).pipe(finalize(() => (this.uploadingDocument = false))).subscribe(
            () => {
                this.notify.info('Pawn ticket document uploaded.');
                this.loadDocuments();
            },
            () => this.notify.error('Unable to upload pawn ticket document.')
        );
        (event.target as HTMLInputElement).value = '';
    }

    deleteDocument(document: PawnTicketDocument): void {
        if (!document.id || !confirm(`Delete ${document.originalFileName}?`)) return;
        this.documentService.delete(document.id).subscribe(() => {
            this.revokeDocumentUrl(document);
            this.notify.info('Pawn ticket document deleted.');
            this.loadDocuments();
        });
    }

    isImage(document: PawnTicketDocument): boolean {
        return !!document.contentType?.startsWith('image/');
    }

    private loadDocumentUrl(document: PawnTicketDocument): void {
        if (!document.id || this.documentUrls.has(document.id)) return;
        this.documentService.download(document.id).subscribe((response) => {
            this.documentUrls.set(document.id, URL.createObjectURL(response.data));
            this.cd.detectChanges();
        });
    }

    private revokeDocumentUrl(document: PawnTicketDocument): void {
        if (!document.id) return;
        const url = this.documentUrls.get(document.id);
        if (url) URL.revokeObjectURL(url);
        this.documentUrls.delete(document.id);
    }

    selectTicket(): void {
        const selected = this.ticketOptions.find((ticket) => ticket.id === this.selectedTicketId);
        if (selected) {
            this.ticketService.getViaIdForEdit(selected.id).subscribe((result) => {
                this.ticket = result.pawnTicket || this.ticket;
                this.selectCustomer();
                this.pawnItemService.getAllViaPawnTicketId(selected.id).subscribe((items) => {
                    this.items = (items || []).map((item) => new CreateOrEditPawnItemDto({ ...item, id: undefined }));
                    this.items.forEach((item) => this.updateItemDescription(item));
                    if (this.items.length === 0) this.addItem();
                });
            });
        } else {
            this.ticket.id = 0;
            this.ticket.ticketNo = undefined;
        }
    }

    addItem(): void {
        const item = new CreateOrEditPawnItemDto();
        item.quantity = 1;
        item.weight = 0;
        item.length = 0;
        item.includedItemWeight = 0;
        item.includedItemValue = 0;
        this.items.push(item);
    }

    removeItem(index: number): void {
        this.items.splice(index, 1);
        this.calculate();
    }

    calculate(): void {
        const changed = this.items.some((item) => item.weight > 0 && item.goldType);
        if (!changed || this.calculating) return;
        this.calculating = true;
        const payload = new CreatePawnTicketWithItemsDto({ ticket: this.ticket, items: this.items });
        this.ticketService.dynamicCalculate(payload).pipe(finalize(() => (this.calculating = false))).subscribe((result) => {
            this.ticket = result;
            this.cd.detectChanges();
        });
    }

    save(): void {
        if (!this.ticket.customer) {
            this.notify.warn('Please select a customer.');
            return;
        }
        if (!this.ticket.generalSetup) {
            this.notify.warn('Please select an outlet.');
            return;
        }
        if (!this.ticket.pledgedDate) {
            this.notify.warn('Please select a pledged date.');
            return;
        }
        if (!this.ticket.amount || this.ticket.amount <= 0) {
            this.notify.warn('Please enter an amount greater than zero.');
            return;
        }
        if (this.items.length === 0) {
            this.notify.warn('Please add at least one pawn item.');
            return;
        }
        this.saving = true;
        this.items.forEach((item) => (item.pawnTicket = this.ticket.id || undefined));
        const payload = new CreatePawnTicketWithItemsDto({ ticket: this.ticket, items: this.items });
        this.ticketService.createTicketAndPawnItem(payload).pipe(finalize(() => (this.saving = false))).subscribe(() => {
            this.notify.info(this.l('SavedSuccessfully'));
            this.modal.hide();
            this.loadTickets();
        });
    }

    resetForm(): void {
        this.ticket = new CreateOrEditPawnTicketDto();
        this.ticket.weight = 0;
        this.ticket.value = 0;
        this.ticket.amount = 0;
        this.ticket.monthlyCustody = 0;
        this.ticket.serviceCharge = 0;
        this.ticket.pledgedDate = (window as any).moment();
        this.items = [];
        this.payments = [];
        this.newPaymentAmount = 0;
        this.newPaymentMethod = undefined;
        this.selectedTicketId = undefined;
        this.selectedCustomer = undefined;
        this.addItem();
    }
}
