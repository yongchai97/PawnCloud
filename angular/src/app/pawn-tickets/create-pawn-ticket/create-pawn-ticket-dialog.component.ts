import { Component, Injector, OnInit, ChangeDetectorRef, EventEmitter, output } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { PawnTicketServiceProxy, CreateOrEditPawnTicketDto, MiscFunctionServiceProxy, StatusDto, CreateOrEditPawnItemDto } from '@shared/service-proxies/service-proxies';
import { LookupServiceProxy, CustomerLookupDto } from '@shared/service-proxies/lookup-service-proxy';
import { NgFor, NgIf } from '@angular/common';
import moment from 'moment';

@Component({
    templateUrl: './create-pawn-ticket-dialog.component.html',
    standalone: true,
    imports: [
        FormsModule,
        AbpModalHeaderComponent,
        AbpValidationSummaryComponent,
        AbpModalFooterComponent,
        LocalizePipe,
        NgFor,
        NgIf,
    ],
})
export class CreatePawnTicketDialogComponent extends AppComponentBase implements OnInit {
    saving = false;
    pawnTicket = new CreateOrEditPawnTicketDto();
    customers: CustomerLookupDto[] = [];
    statuses: StatusDto[] = [];

    // Pawn items to be created together with this ticket
    pawnItems: CreateOrEditPawnItemDto[] = [];

    // Local Date fields used for the native date input
    createdDate: string = this.todayIso();
    maturityDate: string = '';
    expiryDate: string = '';

    onSave = output<EventEmitter<any>>();

    constructor(
        injector: Injector,
        private _lookupService: LookupServiceProxy,
        private _miscService: MiscFunctionServiceProxy,
        private _modalService: BsModalService,
        public bsModalRef: BsModalRef,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    private todayIso(): string {
        const d = new Date();
        const m = (d.getMonth() + 1).toString().padStart(2, '0');
        const day = d.getDate().toString().padStart(2, '0');
        return `${d.getFullYear()}-${m}-${day}`;
    }

    ngOnInit(): void {
        if (!this.pawnTicket.status) {
            this.pawnTicket.status = 'Pending';
        }
        this._lookupService.getCustomersForLookup().subscribe((result) => {
            this.customers = result.items || [];
            // Force change detection: the HTTP response can land outside the
            // zone the modal lives in, so the <select> wouldn't re-render
            // its <option> children without an explicit tick.
            this.cd.detectChanges();
        });
        this._miscService.getStatuses().subscribe((result) => {
            this.statuses = result.items || [];
            this.cd.detectChanges();
        });
    }

    addPawnItem(): void {
        // Open popup with empty form; only commit when the user actually clicked Save.
        import('../shared/pawn-item-form-dialog.component').then((m) => {
            const dto = new CreateOrEditPawnItemDto();
            const dialog = this._modalService.show(m.PawnItemFormDialogComponent, {
                class: 'modal-lg',
                initialState: { item: dto, isEdit: false },
            });
            dialog.onHidden.subscribe(() => {
                const saved = dialog.content && (dialog.content as any).saved;
                const result = dialog.content && (dialog.content as any).item;
                if (saved && result) {
                    this.pawnItems.push(result);
                    this.cd.detectChanges();
                }
            });
        });
    }

    editPawnItem(index: number): void {
        // Open popup pre-populated with the item's data; only commit on Save.
        import('../shared/pawn-item-form-dialog.component').then((m) => {
            const clone = Object.assign(new CreateOrEditPawnItemDto(), this.pawnItems[index]);
            const dialog = this._modalService.show(m.PawnItemFormDialogComponent, {
                class: 'modal-lg',
                initialState: { item: clone, isEdit: true },
            });
            dialog.onHidden.subscribe(() => {
                const saved = dialog.content && (dialog.content as any).saved;
                const result = dialog.content && (dialog.content as any).item;
                if (saved && result) {
                    this.pawnItems[index] = result;
                    this.cd.detectChanges();
                }
            });
        });
    }

    removePawnItem(index: number): void {
        this.pawnItems.splice(index, 1);
        this.cd.detectChanges();
    }

    save(): void {
        this.saving = true;
        this.pawnTicket.createdDate = moment(this.createdDate || undefined) as any;
        this.pawnTicket.maturityDate = (this.maturityDate ? moment(this.maturityDate) : undefined) as any;
        this.pawnTicket.expiryDate = (this.expiryDate ? moment(this.expiryDate) : undefined) as any;
        // Clear any stray pawnTicketId from items; backend will set it
        this.pawnItems.forEach((it) => (it.pawnTicketId = 0));

        this._lookupService
            .createPawnTicketWithItems({ ticket: this.pawnTicket, items: this.pawnItems })
            .subscribe(
                () => this.finish(),
                () => {
                    this.saving = false;
                }
            );
    }

    private finish(): void {
        this.notify.info(this.l('SavedSuccessfully'));
        this.bsModalRef.hide();
        this.onSave.emit(null);
    }
}