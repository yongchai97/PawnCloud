import { Component, Injector, OnInit, ChangeDetectorRef, EventEmitter, output } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
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
        NgFor,
        NgIf,
    ],
})
export class EditPawnTicketDialogComponent extends AppComponentBase implements OnInit {
    saving = false;
    pawnTicket = new CreateOrEditPawnTicketDto();
    customers: CustomerLookupDto[] = [];
    statuses: StatusDto[] = [];
    pawnItems: PawnItemRow[] = [];
    id?: number;

    // Local Date fields (yyyy-MM-dd) used for the native date input.
    createdDate: string = '';
    maturityDate: string = '';
    expiryDate: string = '';

    onSave = output<EventEmitter<any>>();

    constructor(
        injector: Injector,
        private _pawnTicketService: PawnTicketServiceProxy,
        private _pawnItemService: PawnItemServiceProxy,
        private _miscService: MiscFunctionServiceProxy,
        private _lookupService: LookupServiceProxy,
        private _modalService: BsModalService,
        public bsModalRef: BsModalRef,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    private toIsoDate(value: any): string {
        if (!value) return '';
        const d = moment(value);
        if (!d.isValid()) return '';
        return d.format('YYYY-MM-DD');
    }

    ngOnInit(): void {
        this._lookupService.getCustomersForLookup().subscribe((result) => {
            this.customers = result.items || [];
            // Force change detection so the <select> rerenders its <option>
            // children after the async fetch completes.
            this.cd.detectChanges();
        });
        this._miscService.getStatuses().subscribe((result) => {
            this.statuses = result.items || [];
            this.cd.detectChanges();
        });

        if (this.id) {
            this._pawnTicketService.getViaIdForEdit(this.id).subscribe((result: GetPawnTicketForEditOutput) => {
                this.pawnTicket = result.pawnTicket;
                this.createdDate = this.toIsoDate(this.pawnTicket.createdDate);
                this.maturityDate = this.toIsoDate(this.pawnTicket.maturityDate);
                this.expiryDate = this.toIsoDate(this.pawnTicket.expiryDate);
                this.loadItems();
                this.cd.detectChanges();
            });
        }
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
            // In zoneless change detection, mutating the array isn't enough
            // to refresh the *ngFor; the template needs an explicit tick.
            this.cd.detectChanges();
        });
    }

    addPawnItem(): void {
        // Open popup with empty form; on hide, append the returned item to the list
        // ONLY if the user actually clicked Save (the dialog sets saved=true).
        // Lazy import to avoid circular dependency at module-evaluation time.
        import('../shared/pawn-item-form-dialog.component').then((m) => {
            const dto = new CreateOrEditPawnItemDto();
            dto.pawnTicketId = this.id;
            const dialog = this._modalService.show(m.PawnItemFormDialogComponent, {
                class: 'modal-lg',
                initialState: { item: dto, isEdit: false },
            });
            dialog.onHidden.subscribe(() => {
                const saved = dialog.content && (dialog.content as any).saved;
                const result = dialog.content && (dialog.content as any).item;
                if (saved && result) {
                    this.pawnItems.push({
                        isExisting: false,
                        markedForDeletion: false,
                        data: result,
                    });
                    this.cd.detectChanges();
                }
            });
        });
    }

    editPawnItem(index: number): void {
        const row = this.pawnItems[index];
        if (row.markedForDeletion) {
            return;
        }
        // Open popup pre-populated with the item's data; only commit on Save.
        import('../shared/pawn-item-form-dialog.component').then((m) => {
            const clone = Object.assign(new CreateOrEditPawnItemDto(), row.data);
            const dialog = this._modalService.show(m.PawnItemFormDialogComponent, {
                class: 'modal-lg',
                initialState: { item: clone, isEdit: true },
            });
            dialog.onHidden.subscribe(() => {
                const saved = dialog.content && (dialog.content as any).saved;
                const result = dialog.content && (dialog.content as any).item;
                if (saved && result) {
                    row.data = Object.assign(new CreateOrEditPawnItemDto(), result);
                    this.cd.detectChanges();
                }
            });
        });
    }

    removePawnItem(index: number): void {
        const row = this.pawnItems[index];
        if (row.isExisting) {
            row.markedForDeletion = true;
        } else {
            this.pawnItems.splice(index, 1);
        }
        this.cd.detectChanges();
    }

    undoDelete(index: number): void {
        this.pawnItems[index].markedForDeletion = false;
        this.cd.detectChanges();
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