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
    templateUrl: './edit-pawn-item-dialog.component.html',
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
export class EditPawnItemDialogComponent extends AppComponentBase implements OnInit {
    saving = false;
    pawnItem = new CreateOrEditPawnItemDto();
    id?: number;

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
        if (this.id) {
            this._lookupService.getPawnItemForEdit(this.id).subscribe((result) => {
                // The ABP envelope wraps { pawnItem: ... }; unwrap if present.
                const dto = (result && result.pawnItem) ? result.pawnItem : result;
                this.pawnItem = Object.assign(new CreateOrEditPawnItemDto(), dto);
                this.cd.detectChanges();
            });
        }
    }

    save(): void {
        this.saving = true;
        this.pawnItem.id = this.id;

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
