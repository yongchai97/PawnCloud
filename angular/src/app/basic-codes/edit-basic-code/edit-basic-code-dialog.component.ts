import { Component, Injector, ChangeDetectorRef, EventEmitter, output, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CreateOrEditBasicCodeDto, BasicCodeServiceProxy, MiscMasterConfigServiceProxy, MiscMasterConfigLookupDto } from '@shared/service-proxies/service-proxies';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { DropdownModule } from 'primeng/dropdown';
import { NgFor } from '@angular/common';

@Component({
    templateUrl: './edit-basic-code-dialog.component.html',
    standalone: true,
    imports: [FormsModule, AbpModalHeaderComponent, AbpValidationSummaryComponent, AbpModalFooterComponent, LocalizePipe, InputTextModule, InputNumberModule, ButtonModule, CheckboxModule, DropdownModule, NgFor],
})
export class EditBasicCodeDialogComponent extends AppComponentBase implements OnInit {
    saving = false;
    basicCode = new CreateOrEditBasicCodeDto();
    miscMasterConfigOptions: MiscMasterConfigLookupDto[] = [];
    id: number;

    onSave = output<EventEmitter<any>>();

    constructor(
        injector: Injector,
        private _basicCodeService: BasicCodeServiceProxy,
        private _miscMasterConfigService: MiscMasterConfigServiceProxy,
        public bsModalRef: BsModalRef,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.loadMiscMasterConfigOptions();
        this.loadBasicCode();
    }

    loadBasicCode(): void {
        if (this.id) {
            this._basicCodeService.getViaIdForEdit(this.id).subscribe((result) => {
                this.basicCode = result.basicCode;
                this.cd.detectChanges();
            });
        }
    }

    loadMiscMasterConfigOptions(): void {
        this._miscMasterConfigService.getForLookup().subscribe((result) => {
            this.miscMasterConfigOptions = result.items || [];
            this.cd.detectChanges();
        });
    }

    save(): void {
        this.saving = true;
        this._basicCodeService.createOrEdit(this.basicCode).subscribe(
            () => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.bsModalRef.hide();
                this.onSave.emit(null);
            },
            () => {
                this.saving = false;
                this.cd.detectChanges();
            }
        );
    }
}
