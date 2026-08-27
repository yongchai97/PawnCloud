import { Component, ChangeDetectorRef, Injector, TemplateRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { LookupServiceProxy, CustomerLookupDto, PawnTicketLookupDto } from '@shared/service-proxies/lookup-service-proxy';
import {
    BasicCodeLookupDto, BasicCodeServiceProxy, CreateOrEditPawnItemDto, CreateOrEditPawnTicketDto,
    CreatePawnTicketWithItemsDto,
    GoldType, GoldTypeServiceProxy, ItemListing, ItemListingServiceProxy, ItemStatus,
    ItemStatusServiceProxy, PawnItemServiceProxy, PawnTicketDto, PawnTicketServiceProxy,
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
    customers: CustomerLookupDto[] = [];
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
    @ViewChild('editor') editor!: TemplateRef<any>;

    constructor(
        injector: Injector,
        private lookup: LookupServiceProxy,
        private ticketService: PawnTicketServiceProxy,
        private listingService: ItemListingServiceProxy,
        private statusService: ItemStatusServiceProxy,
        private goldService: GoldTypeServiceProxy,
        private basicCodeService: BasicCodeServiceProxy,
        private pawnItemService: PawnItemServiceProxy,
        private modal: BsModalService,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
        this.resetForm();
    }

    ngOnInit(): void {
        this.loadTickets();
        this.loadCustomers();
        this.listingService.getAll().subscribe((items) => {
            this.itemListings = items || [];
            this.itemListingOptions = this.itemListings.map((item) => ({ id: item.id, label: [item.code, item.description].filter(Boolean).join(' - ') }));
        });
        this.statusService.getAll().subscribe((items) => {
            this.itemStatuses = items || [];
            this.itemStatusOptions = this.itemStatuses.map((item) => ({ id: item.id, label: [item.code, item.description].filter(Boolean).join(' - ') }));
        });
        this.goldService.getAll().subscribe((items) => {
            this.goldTypes = items || [];
            this.goldTypeOptions = this.goldTypes.map((item) => ({ id: item.id, label: [item.purity, item.description].filter(Boolean).join(' - ') }));
        });
        this.basicCodeService.getBasicCodesForLookup().subscribe((result) => (this.includedItems = result.items || []));
    }

    loadTickets(): void {
        this.lookup.getPawnTicketsForLookup().subscribe((result) => (this.ticketOptions = result.items || []));
        this.ticketService.getAll('', 'CreationTime DESC', 0, 1000).subscribe((result) => (this.tickets = result.items || []));
    }

    loadCustomers(): void {
        this.lookup.getCustomersForLookup(this.customerFilter).subscribe((result) => (this.customers = result.items || []));
    }

    openCreate(): void {
        this.resetForm();
        this.modal.show(this.editor, { class: 'modal-xl pawn-ticket-modal' });
    }

    selectTicket(): void {
        const selected = this.ticketOptions.find((ticket) => ticket.id === this.selectedTicketId);
        if (selected) {
            this.ticketService.getViaIdForEdit(selected.id).subscribe((result) => {
                this.ticket = result.pawnTicket || this.ticket;
                this.pawnItemService.getAllViaPawnTicketId(selected.id).subscribe((items) => {
                    this.items = (items || []).map((item) => new CreateOrEditPawnItemDto({ ...item, id: undefined }));
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
        if (!this.ticket.customer || this.items.length === 0) return;
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
        this.selectedTicketId = undefined;
        this.addItem();
    }
}
