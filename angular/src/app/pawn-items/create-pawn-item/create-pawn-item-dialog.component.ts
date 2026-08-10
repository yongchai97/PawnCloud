import { Component, Injector, OnInit, ChangeDetectorRef, EventEmitter, output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CreateOrEditPawnItemDto } from '@shared/service-proxies/service-proxies';
import { LookupServiceProxy } from '@shared/service-proxies/lookup-service-proxy';
import { NgIf } from '@angular/common';

@Component({
    templateUrl: './create-pawn-item-dialog.component.html',
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
export class CreatePawnItemDialogComponent extends AppComponentBase implements OnInit {
    saving = false;
    pawnItem = new CreateOrEditPawnItemDto();

    onSave = output<EventEmitter<any>>();

    constructor(
        injector: Injector,
        private _lookupService: LookupServiceProxy,
        public bsModalRef: BsModalRef,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        // Keep the create form empty by default - all DTO fields start undefined.
    }

    save(): void {
        this.saving = true;
        this.pawnItem.id = 0;

        this._lookupService
            .createPawnItem(this.pawnItem)
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
