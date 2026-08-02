import { Component, Injector, ChangeDetectorRef, ApplicationRef, ViewChild, EventEmitter, output, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { PawnTicketServiceProxy, CreateOrEditPawnTicketDto, MiscFunctionServiceProxy, StatusDto, CreateOrEditPawnItemDto } from '@shared/service-proxies/service-proxies';
import { LookupServiceProxy, CustomerLookupDto } from '@shared/service-proxies/lookup-service-proxy';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { ButtonModule } from 'primeng/button';
import { SelectModule, Select } from 'primeng/select';
import { TextareaModule } from 'primeng/textarea';
import { DatePickerModule } from 'primeng/datepicker';
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
        InputTextModule,
        InputNumberModule,
        ButtonModule,
        SelectModule,
        TextareaModule,
        DatePickerModule,
        NgFor,
        NgIf,
    ],
})
export class CreatePawnTicketDialogComponent extends AppComponentBase implements OnInit {
    @ViewChild('customerSelect') customerSelect: Select;
    @ViewChild('statusSelect') statusSelect: Select;

    saving = false;
    pawnTicket = new CreateOrEditPawnTicketDto();
    customers: CustomerLookupDto[] = [];
    statuses: StatusDto[] = [];

    // Pawn items to be created together with this ticket
    pawnItems: CreateOrEditPawnItemDto[] = [];

    // Local Date fields used to bind p-datepicker (DTO uses moment.Moment)
    createdDate: Date | null = new Date();
    maturityDate: Date | null = null;
    expiryDate: Date | null = null;

    onSave = output<EventEmitter<any>>();

    constructor(
        injector: Injector,
        private _lookupService: LookupServiceProxy,
        private _miscService: MiscFunctionServiceProxy,
        public bsModalRef: BsModalRef,
        public cd: ChangeDetectorRef,
        public appRef: ApplicationRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        if (!this.pawnTicket.status) {
            this.pawnTicket.status = 'Pending';
        }
        this._lookupService.getCustomersForLookup().subscribe((result) => {
            this.customers = result.items || [];
            // PrimeNG p-select uses OnPush + signals. With the project in
            // zoneless mode, even appRef.tick() on the parent does not
            // always re-evaluate the option list inside the overlay
            // template outlet. Calling detectChanges() on the Select's
            // own ChangeDetectorRef forces the panel to pick up the new
            // options.
            this.refreshSelect(this.customerSelect);
        });
        this._miscService.getStatuses().subscribe((result) => {
            this.statuses = result.items || [];
            this.refreshSelect(this.statusSelect);
        });
    }

    private refreshSelect(select: Select): void {
        if (!select) return;
        // Force the OnPush select to re-evaluate its template outlet so
        // the new options array is rendered in the panel.
        (select as any).cd?.detectChanges?.();
        this.appRef.tick();
    }

    /**
     * PrimeNG's datepicker hides the overlay via setTimeout(150) AFTER
     * onSelect fires. Calling detectChanges() synchronously in onSelect
     * runs while overlayVisible is still true, so the panel stays.
     * Wait past the 150ms timeout before triggering a tick.
     */
    onDateSelectRefresh(): void {
        setTimeout(() => this.appRef.tick(), 200);
    }

    addPawnItem(): void {
        this.pawnItems.push(new CreateOrEditPawnItemDto());
    }

    removePawnItem(index: number): void {
        this.pawnItems.splice(index, 1);
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
                    this.appRef.tick();
                }
            );
    }

    private finish(): void {
        this.notify.info(this.l('SavedSuccessfully'));
        this.bsModalRef.hide();
        this.onSave.emit(null);
    }
}
