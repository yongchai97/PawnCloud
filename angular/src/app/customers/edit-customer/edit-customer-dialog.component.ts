import { Component, Injector, ChangeDetectorRef, EventEmitter, output, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CustomerServiceProxy, CreateOrEditCustomerDto, GetCustomerForEditOutput, BasicCodeServiceProxy, BasicCodeLookupDto, MiscMasterConfigServiceProxy, MiscMasterConfigLookupDto } from '@shared/service-proxies/service-proxies';
import { LookupServiceProxy } from '@shared/service-proxies/lookup-service-proxy';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { NgIf, NgFor } from '@angular/common';
import { DropdownModule } from 'primeng/dropdown';

@Component({
    templateUrl: './edit-customer-dialog.component.html',
    standalone: true,
    imports: [FormsModule, AbpModalHeaderComponent, AbpValidationSummaryComponent, AbpModalFooterComponent, LocalizePipe, InputTextModule, InputNumberModule, ButtonModule, TableModule, NgIf, NgFor, DropdownModule],
})
export class EditCustomerDialogComponent extends AppComponentBase implements OnInit {
    saving = false;
    customer = new CreateOrEditCustomerDto();
    id?: number;

    // BasicCode dropdowns
    countryOptions: BasicCodeLookupDto[] = [];
    raceOptions: BasicCodeLookupDto[] = [];
    genderOptions: BasicCodeLookupDto[] = [];
    nationalityOptions: BasicCodeLookupDto[] = [];
    businessNatureOptions: BasicCodeLookupDto[] = [];
    maritalStatusOptions: BasicCodeLookupDto[] = [];

    onSave = output<EventEmitter<any>>();

    constructor(
        injector: Injector,
        private _customerService: CustomerServiceProxy,
        private _lookupService: LookupServiceProxy,
        private _basicCodeService: BasicCodeServiceProxy,
        private _miscMasterConfigService: MiscMasterConfigServiceProxy,
        public bsModalRef: BsModalRef,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.loadBasicCodeDropdowns();
        if (this.id) {
            this._customerService.getViaIdForEdit(this.id).subscribe((result: GetCustomerForEditOutput) => {
                this.customer = result.customer;
                this.cd.detectChanges();
            });
        }
    }

    private loadBasicCodeDropdowns(): void {
        this._miscMasterConfigService.getForLookup().subscribe((configs) => {
            const configMap = new Map<string, number>();
            configs.items?.forEach((config) => {
                configMap.set(config.displayName.toLowerCase(), config.id);
            });

            const categoryMap = {
                'country': configMap.get('country'),
                'race': configMap.get('race'),
                'gender': configMap.get('gender'),
                'nationality': configMap.get('nationality'),
                'businessnature': configMap.get('businessnature') || configMap.get('business nature'),
                'maritalstatus': configMap.get('maritalstatus') || configMap.get('marital status'),
            };

            // Load BasicCodes for each category
            if (categoryMap['country']) {
                this._basicCodeService.getBasicCodesByCategory(categoryMap['country']).subscribe((result) => {
                    this.countryOptions = result.items || [];
                    this.cd.detectChanges();
                });
            }
            if (categoryMap['race']) {
                this._basicCodeService.getBasicCodesByCategory(categoryMap['race']).subscribe((result) => {
                    this.raceOptions = result.items || [];
                    this.cd.detectChanges();
                });
            }
            if (categoryMap['gender']) {
                this._basicCodeService.getBasicCodesByCategory(categoryMap['gender']).subscribe((result) => {
                    this.genderOptions = result.items || [];
                    this.cd.detectChanges();
                });
            }
            if (categoryMap['nationality']) {
                this._basicCodeService.getBasicCodesByCategory(categoryMap['nationality']).subscribe((result) => {
                    this.nationalityOptions = result.items || [];
                    this.cd.detectChanges();
                });
            }
            if (categoryMap['businessnature']) {
                this._basicCodeService.getBasicCodesByCategory(categoryMap['businessnature']).subscribe((result) => {
                    this.businessNatureOptions = result.items || [];
                    this.cd.detectChanges();
                });
            }
            if (categoryMap['maritalstatus']) {
                this._basicCodeService.getBasicCodesByCategory(categoryMap['maritalstatus']).subscribe((result) => {
                    this.maritalStatusOptions = result.items || [];
                    this.cd.detectChanges();
                });
            }
        });
    }

    save(): void {
        this.saving = true;
        this.customer.id = this.id;
        this._customerService.createOrEdit(this.customer).subscribe(
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