import { Component, Injector, ChangeDetectorRef, ApplicationRef, ViewChild, EventEmitter, output, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import {
    PawnTicketServiceProxy,
    PawnItemServiceProxy,
    CreateOrEditPawnTicketDto,
    GetPawnTicketForEditOutput,
    MiscFunctionServiceProxy,
    StatusDto,
    CreateOrEditPawnItemDto,
    PawnItemDto,
} from '@shared/service-proxies/service-proxies';
import { LookupServiceProxy, CustomerLookupDto } from '@shared/service-proxies/lookup-service-proxy';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { ButtonModule } from 'primeng/button';
import { SelectModule, Select } from 'primeng/select';
import { TextareaModule } from 'primeng/textarea';
import { DatePickerModule } from 'primeng/datepicker';
import { NgFor, NgIf } from '@angular/common';
import moment from 'moment';

interface PawnItemRow {
    id?: number;
    isExisting: boolean;
    markedForDeletion: boolean;
    data: CreateOrEditPawnItemDto;
}

@Component({
    templateUrl: './edit-pawn-ticket-dialog.component.html',
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
export class EditPawnTicketDialogComponent extends AppComponentBase implements OnInit {
    @ViewChild('customerSelect') customerSelect: Select;
    @ViewChild('statusSelect') statusSelect: Select;

    saving = false;
    pawnTicket = new CreateOrEditPawnTicketDto();
    customers: CustomerLookupDto[] = [];
    statuses: StatusDto[] = [];
    pawnItems: PawnItemRow[] = [];
    id?: number;

    // Local Date fields used to bind p-datepicker (DTO uses moment.Moment)
    createdDate: Date | null = null;
    maturityDate: Date | null = null;
    expiryDate: Date | null = null;

    onSave = output<EventEmitter<any>>();

    constructor(
        injector: Injector,
        private _pawnTicketService: PawnTicketServiceProxy,
        private _pawnItemService: PawnItemServiceProxy,
        private _miscService: MiscFunctionServiceProxy,
        private _lookupService: LookupServiceProxy,
        public bsModalRef: BsModalRef,
        public cd: ChangeDetectorRef,
        public appRef: ApplicationRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this._lookupService.getCustomersForLookup().subscribe((result) => {
            this.customers = result.items || [];
            this.refreshSelect(this.customerSelect);
        });
        this._miscService.getStatuses().subscribe((result) => {
            this.statuses = result.items || [];
            this.refreshSelect(this.statusSelect);
        });

        if (this.id) {
            this._pawnTicketService.getViaIdForEdit(this.id).subscribe((result: GetPawnTicketForEditOutput) => {
                this.pawnTicket = result.pawnTicket;
                this.createdDate = this.pawnTicket.createdDate ? (this.pawnTicket.createdDate as any).toDate() : null;
                this.maturityDate = this.pawnTicket.maturityDate ? (this.pawnTicket.maturityDate as any).toDate() : null;
                this.expiryDate = this.pawnTicket.expiryDate ? (this.pawnTicket.expiryDate as any).toDate() : null;
                this.loadItems();
                this.appRef.tick();
                this.refreshSelect(this.customerSelect);
                this.refreshSelect(this.statusSelect);
            });
        }
    }

    private refreshSelect(select: Select): void {
        if (!select) return;
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

    private loadItems(): void {
        if (!this.id) return;
        this._lookupService.getPawnItemsByPawnTicketId(this.id).subscribe((result) => {
            this.pawnItems = (result.items || []).map((dto: PawnItemDto) => {
                const itemDto = new CreateOrEditPawnItemDto();
                itemDto.id = dto.id;
                itemDto.pawnTicketId = dto.pawnTicketId;
                itemDto.category = dto.category;
                itemDto.description = dto.description;
                itemDto.weight = dto.weight;
                itemDto.purity = dto.purity;
                itemDto.serialNumber = dto.serialNumber;
                itemDto.estimatedValue = dto.estimatedValue;
                itemDto.marketValue = dto.marketValue;
                itemDto.loanValue = dto.loanValue;
                itemDto.condition = dto.condition;
                return {
                    id: dto.id,
                    isExisting: true,
                    markedForDeletion: false,
                    data: itemDto,
                };
            });
            this.appRef.tick();
        });
    }

    addPawnItem(): void {
        const dto = new CreateOrEditPawnItemDto();
        dto.pawnTicketId = this.id;
        this.pawnItems.push({
            isExisting: false,
            markedForDeletion: false,
            data: dto,
        });
    }

    removePawnItem(index: number): void {
        const row = this.pawnItems[index];
        if (row.isExisting) {
            row.markedForDeletion = true;
        } else {
            this.pawnItems.splice(index, 1);
        }
        this.appRef.tick();
    }

    undoDelete(index: number): void {
        const pawnItems = this.pawnItems;
        pawnItems[index].markedForDeletion = false;
        this.appRef.tick();
    }

    save(): void {
        this.saving = true;
        this.pawnTicket.id = this.id;
        this.pawnTicket.createdDate = moment(this.createdDate || undefined) as any;
        this.pawnTicket.maturityDate = (this.maturityDate ? moment(this.maturityDate) : undefined) as any;
        this.pawnTicket.expiryDate = (this.expiryDate ? moment(this.expiryDate) : undefined) as any;

        this._pawnTicketService.createOrEdit(this.pawnTicket).subscribe(
            () => this.processItems(),
            () => {
                this.saving = false;
                this.appRef.tick();
            }
        );
    }

    private processItems(): void {
        const deletes = this.pawnItems.filter((r) => r.isExisting && r.markedForDeletion && r.id);
        const saves = this.pawnItems.filter((r) => !r.markedForDeletion);

        let remaining = deletes.length + saves.length;
        if (remaining === 0) {
            this.finish();
            return;
        }
        let hadError = false;
        const onDone = () => {
            remaining--;
            if (remaining === 0) {
                if (hadError) {
                    this.saving = false;
                    this.appRef.tick();
                } else {
                    this.finish();
                }
            }
        };
        deletes.forEach((row) => {
            this._lookupService.deletePawnItem(row.id!).subscribe(
                () => onDone(),
                () => {
                    hadError = true;
                    onDone();
                }
            );
        });
        saves.forEach((row) => {
            row.data.pawnTicketId = this.id;
            this._pawnItemService.createOrEdit(row.data).subscribe(
                () => onDone(),
                () => {
                    hadError = true;
                    onDone();
                }
            );
        });
    }

    private finish(): void {
        this.notify.info(this.l('SavedSuccessfully'));
        this.bsModalRef.hide();
        this.onSave.emit(null);
    }
}
