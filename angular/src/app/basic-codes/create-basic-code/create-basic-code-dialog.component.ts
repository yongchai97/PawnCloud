import { Component, Injector, ChangeDetectorRef, EventEmitter, output, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CreateOrEditBasicCodeDto, BasicCodeServiceProxy, MiscMasterConfigLookupDto } from '@shared/service-proxies/service-proxies';
import { LookupServiceProxy } from '@shared/service-proxies/lookup-service-proxy';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { NgFor } from '@angular/common';

@Component({
    templateUrl: './create-basic-code-dialog.component.html',
    standalone: true,
    imports: [FormsModule, AbpModalHeaderComponent, AbpValidationSummaryComponent, AbpModalFooterComponent, LocalizePipe, InputTextModule, InputNumberModule, ButtonModule, DropdownModule, NgFor],
})
export class CreateBasicCodeDialogComponent extends AppComponentBase implements OnInit {
    saving = false;
    basicCode = new CreateOrEditBasicCodeDto();
    miscMasterConfigOptions: MiscMasterConfigLookupDto[] = [];

    onSave = output<EventEmitter<any>>();

    constructor(
        injector: Injector,
        private _basicCodeService: BasicCodeServiceProxy,
        private _lookupService: LookupServiceProxy,
        public bsModalRef: BsModalRef,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
        this.basicCode.systemProvidedValue = false;
    }

    ngOnInit(): void {
        this.loadMiscMasterConfigOptions();
    }

    loadMiscMasterConfigOptions(): void {
        this._lookupService.getMiscMasterConfigs().subscribe((result) => {
            this.miscMasterConfigOptions = (result.items || []).filter((item) => item.availableForUser);
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
