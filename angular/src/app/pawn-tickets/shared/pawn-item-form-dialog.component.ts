import { Component, Injector } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CreateOrEditPawnItemDto } from '@shared/service-proxies/service-proxies';
import { NgIf } from '@angular/common';

@Component({
    selector: 'pawn-item-form-dialog',
    templateUrl: './pawn-item-form-dialog.component.html',
    standalone: true,
    imports: [
        FormsModule,
        AbpModalHeaderComponent,
        AbpValidationSummaryComponent,
        AbpModalFooterComponent,
        LocalizePipe,
        NgIf,
    ],
})
export class PawnItemFormDialogComponent extends AppComponentBase {
    saving = false;
    saved = false;
    item = new CreateOrEditPawnItemDto();
    isEdit = false;

    constructor(
        injector: Injector,
        public bsModalRef: BsModalRef
    ) {
        super(injector);
    }

    save(): void {
        this.saving = true;
        this.saved = true;
        // Return the current item to the caller via the BsModalRef content.
        this.bsModalRef.content.item = this.item;
        this.bsModalRef.hide();
    }
}
