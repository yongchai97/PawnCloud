import { Component, EventEmitter, Injector, OnInit, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { NgFor } from '@angular/common';
import { AppComponentBase } from '@shared/app-component-base';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import {
    BasicCodeLookupDto,
    BasicCodeServiceProxy,
    CreateOrEditPawnTicketDto,
    GoldType,
    GoldTypeServiceProxy,
    ItemListing,
    ItemListingServiceProxy,
    ItemStatus,
    ItemStatusServiceProxy,
    PawnTicketServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { CustomerLookupDto, LookupServiceProxy } from '@shared/service-proxies/lookup-service-proxy';

@Component({
    templateUrl: './create-pawn-ticket-dialog.component.html',
    standalone: true,
    imports: [FormsModule, NgFor, AbpModalHeaderComponent, AbpValidationSummaryComponent, AbpModalFooterComponent, LocalizePipe],
})
export class CreatePawnTicketDialogComponent extends AppComponentBase implements OnInit {
    saving = false;
    pawnTicket = new CreateOrEditPawnTicketDto();
    customers: CustomerLookupDto[] = [];
    itemListings: ItemListing[] = [];
    itemStatuses: ItemStatus[] = [];
    goldTypes: GoldType[] = [];
    includedItems: BasicCodeLookupDto[] = [];
    onSave = output<EventEmitter<any>>();

    constructor(
        injector: Injector,
        private pawnTicketService: PawnTicketServiceProxy,
        private lookupService: LookupServiceProxy,
        private itemListingService: ItemListingServiceProxy,
        private itemStatusService: ItemStatusServiceProxy,
        private goldTypeService: GoldTypeServiceProxy,
        private basicCodeService: BasicCodeServiceProxy,
        public bsModalRef: BsModalRef
    ) {
        super(injector);
        this.pawnTicket.weight = 0;
        this.pawnTicket.length = 0;
        this.pawnTicket.value = 0;
        this.pawnTicket.includedItemWeight = 0;
    }

    ngOnInit(): void {
        this.lookupService.getCustomersForLookup().subscribe((result) => (this.customers = result.items || []));
        this.itemListingService.getAll().subscribe((result) => (this.itemListings = result || []));
        this.itemStatusService.getAll().subscribe((result) => (this.itemStatuses = result || []));
        this.goldTypeService.getAll().subscribe((result) => (this.goldTypes = result || []));
        this.basicCodeService.getBasicCodesForLookup().subscribe((result) => {
            this.includedItems = result.items || [];
        });
    }

    save(): void {
        this.saving = true;
        this.pawnTicketService.createOrEdit(this.pawnTicket).subscribe(
            () => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.bsModalRef.hide();
                this.onSave.emit(null);
            },
            () => (this.saving = false)
        );
    }
}
