import { Component, Injector, ChangeDetectorRef, EventEmitter, output, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CreateOrEditCustomerDto, BasicCodeServiceProxy, BasicCodeLookupDto, Country, CountryServiceProxy, MiscMasterConfigServiceProxy, MiscMasterConfigLookupDto } from '@shared/service-proxies/service-proxies';
import { LookupServiceProxy } from '@shared/service-proxies/lookup-service-proxy';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { NgFor } from '@angular/common';
import moment from 'moment';

@Component({
    templateUrl: './create-customer-dialog.component.html',
    standalone: true,
    imports: [FormsModule, AbpModalHeaderComponent, AbpValidationSummaryComponent, AbpModalFooterComponent, LocalizePipe, InputTextModule, InputNumberModule, ButtonModule, DropdownModule, NgFor],
})
export class CreateCustomerDialogComponent extends AppComponentBase implements OnInit {
    saving = false;
    customer = new CreateOrEditCustomerDto();

    // BasicCode dropdowns
    countryOptions: { label: string; value: number }[] = [];
    raceOptions: BasicCodeLookupDto[] = [];
    genderOptions: BasicCodeLookupDto[] = [];
    nationalityOptions: { label: string; value: number }[] = [];
    businessNatureOptions: BasicCodeLookupDto[] = [];
    maritalStatusOptions: BasicCodeLookupDto[] = [];

    onSave = output<EventEmitter<any>>();

    constructor(
        injector: Injector,
        private _lookupService: LookupServiceProxy,
        private _basicCodeService: BasicCodeServiceProxy,
        private _countryService: CountryServiceProxy,
        private _miscMasterConfigService: MiscMasterConfigServiceProxy,
        public bsModalRef: BsModalRef,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
        this.customer.age = 0;
    }

    ngOnInit(): void {
        this.loadCountries();
        this.loadBasicCodeDropdowns();
    }

    private loadCountries(): void {
        this._countryService.getAllViaYear(new Date().getFullYear()).subscribe((countries: Country[]) => {
            this.countryOptions = (countries || []).map((country) => ({
                label: `${country.countryCodeThreeAlphabet} - ${country.countryName}`,
                value: country.id,
            }));
            this.nationalityOptions = this.countryOptions;
            this.cd.detectChanges();
        });
    }

    private loadBasicCodeDropdowns(): void {
        this._lookupService.getMiscMasterConfigs().subscribe((configs) => {
            const configMap = new Map<string, number>();
            configs.items?.forEach((config) => {
                configMap.set(config.displayName.trim().toLowerCase().replace(/\s+/g, ' '), config.id);
            });

            const categoryMap = {
                'race': configMap.get('race'),
                'gender': configMap.get('gender'),
                'businessnature': configMap.get('business nature') || configMap.get('businessnature'),
                'maritalstatus': configMap.get('marital status') || configMap.get('maritalstatus'),
            };

            // Load BasicCodes for each category
            if (categoryMap['race']) {
                this._lookupService.getBasicCodesByCategory(categoryMap['race']).subscribe((result) => {
                    this.raceOptions = result.items || [];
                    this.cd.detectChanges();
                });
            }
            if (categoryMap['gender']) {
                this._lookupService.getBasicCodesByCategory(categoryMap['gender']).subscribe((result) => {
                    this.genderOptions = result.items || [];
                    this.cd.detectChanges();
                });
            }
            if (categoryMap['businessnature']) {
                this._lookupService.getBasicCodesByCategory(categoryMap['businessnature']).subscribe((result) => {
                    this.businessNatureOptions = result.items || [];
                    this.cd.detectChanges();
                });
            }
            if (categoryMap['maritalstatus']) {
                this._lookupService.getBasicCodesByCategory(categoryMap['maritalstatus']).subscribe((result) => {
                    this.maritalStatusOptions = result.items || [];
                    this.cd.detectChanges();
                });
            }
        });
    }

    onDropdownChange(): void {
        this.cd.detectChanges();
    }

    readBirthDateFromNric(): void {
        const digits = (this.customer.nric || '').replace(/\D/g, '');
        if (digits.length < 6) {
            return;
        }

        const currentYearTwoDigits = new Date().getFullYear() % 100;
        const year = Number(digits.substring(0, 2));
        const fullYear = year > currentYearTwoDigits ? 1900 + year : 2000 + year;
        const month = digits.substring(2, 4);
        const day = digits.substring(4, 6);
        const birthDate = moment(`${fullYear}-${month}-${day}`, 'YYYY-MM-DD', true);

        if (birthDate.isValid()) {
            this.customer.birthDate = birthDate.format('YYYY-MM-DD') as any;
            this.updateAge();
        }
    }

    isUnderage(): boolean {
        return !!this.customer.birthDate && this.customer.age < 18;
    }

    updateAge(): void {
        const value = this.customer.birthDate as any;
        if (!value) {
            this.customer.age = 0;
            return;
        }
        const birthDate = moment.isMoment(value) ? value.toDate() : new Date(`${value}T00:00:00`);
        const age = (Date.now() - birthDate.getTime()) / (365.25 * 24 * 60 * 60 * 1000);
        this.customer.age = Number.isFinite(age) && age >= 0 ? Math.floor(age) : 0;
        this.cd.detectChanges();
    }

    copyAddressToMailing(): void {
        this.customer.mailingAddress = this.customer.address;
        this.customer.mailingCity = this.customer.city;
        this.customer.mailingState = this.customer.state;
        this.customer.mailingPostcode = this.customer.postcode;
        this.customer.mailingCountry = this.customer.country;
        this.cd.detectChanges();
    }

    save(): void {
        if (this.isUnderage()) {
            this.notify.error('Minimum Age for The Operation is 18 Years Old');
            return;
        }

        this.saving = true;
        const birthDate = this.customer.birthDate as any;
        this.customer.birthDate = birthDate ? (moment.isMoment(birthDate) ? birthDate : moment(birthDate)) : (undefined as any);
        this._lookupService.createCustomer(this.customer).subscribe(
            (newId) => {
                this.customer.id = newId;
                this.notify.info(this.l('SavedSuccessfully'));
                this.bsModalRef.hide();
                this.onSave.emit(null);
            },
            (error) => {
                this.saving = false;
                const message = error?.error?.error?.message || error?.error?.message || error?.message || 'Unable to save customer.';
                this.notify.error(message);
                this.cd.detectChanges();
            }
        );
    }
}