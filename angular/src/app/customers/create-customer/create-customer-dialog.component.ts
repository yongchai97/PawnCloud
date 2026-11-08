import { Component, Injector, ChangeDetectorRef, EventEmitter, output, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { BsModalService } from 'ngx-bootstrap/modal';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CreateOrEditCustomerDto, CreateOrEditCustomerCDDDto, SearchCDDDto, CustomerCDDServiceProxy, BasicCodeServiceProxy, BasicCodeLookupDto, Country, CountryServiceProxy, MiscMasterConfigServiceProxy, MiscMasterConfigLookupDto, PawnTicketServiceProxy } from '@shared/service-proxies/service-proxies';
import { LookupServiceProxy } from '@shared/service-proxies/lookup-service-proxy';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { NgFor } from '@angular/common';
import moment from 'moment';
import { HighRiskCustomerWarningDialogComponent } from '../high-risk-customer-warning-dialog.component';

@Component({
    templateUrl: './create-customer-dialog.component.html',
    standalone: true,
    imports: [FormsModule, AbpModalHeaderComponent, AbpValidationSummaryComponent, AbpModalFooterComponent, LocalizePipe, InputTextModule, InputNumberModule, ButtonModule, DropdownModule, NgFor, TabsModule],
})
export class CreateCustomerDialogComponent extends AppComponentBase implements OnInit {
    saving = false;
    customer = new CreateOrEditCustomerDto();
    customerCDD = new CreateOrEditCustomerCDDDto();
    checkingCDD = false;
    private lastCheckedCustomerName = '';

    // BasicCode dropdowns
    countryOptions: { label: string; value: number }[] = [];
    raceOptions: BasicCodeLookupDto[] = [];
    genderOptions: BasicCodeLookupDto[] = [];
    nationalityOptions: { label: string; value: number }[] = [];
    maritalStatusOptions: BasicCodeLookupDto[] = [];
    paymentModeOptions: BasicCodeLookupDto[] = [];
    deliveryChannelOptions: BasicCodeLookupDto[] = [];

    onSave = output<EventEmitter<any>>();

    constructor(
        injector: Injector,
        private _lookupService: LookupServiceProxy,
        private _basicCodeService: BasicCodeServiceProxy,
        private _countryService: CountryServiceProxy,
        private _miscMasterConfigService: MiscMasterConfigServiceProxy,
        private _customerCDDService: CustomerCDDServiceProxy,
        private _pawnTicketService: PawnTicketServiceProxy,
        private _modalService: BsModalService,
        public bsModalRef: BsModalRef,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
        this.customer.age = 0;
        this.customer.monthlyIncome = 0;
    }

    ngOnInit(): void {
        this.loadCountries();
        this.loadBasicCodeDropdowns();
        this.customerCDD.approvedBy = this.appSession.userId;
        this.customerCDD.pepStatus = false;
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
                'maritalstatus': configMap.get('marital status') || configMap.get('maritalstatus'),
                'paymentmethod': configMap.get('payment method') || configMap.get('paymentmode'),
                'deliverychannel': configMap.get('delivery channel') || configMap.get('deliverychannel'),
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
            if (categoryMap['maritalstatus']) {
                this._lookupService.getBasicCodesByCategory(categoryMap['maritalstatus']).subscribe((result) => {
                    this.maritalStatusOptions = result.items || [];
                    this.cd.detectChanges();
                });
            }
            if (categoryMap['paymentmethod']) {
                this._lookupService.getBasicCodesByCategory(categoryMap['paymentmethod']).subscribe((result) => {
                    this.paymentModeOptions = result.items || [];
                    this.cd.detectChanges();
                });
            }
            if (categoryMap['deliverychannel']) {
                this._lookupService.getBasicCodesByCategory(categoryMap['deliverychannel']).subscribe((result) => {
                    this.deliveryChannelOptions = result.items || [];
                    this.cd.detectChanges();
                });
            }
        });
    }

    onDropdownChange(): void {
        this.cd.detectChanges();
    }

    onPepStatusChange(): void {
        if (!this.customerCDD.pepStatus) {
            this.customerCDD.partyName = undefined;
            this.customerCDD.roleInParty = undefined;
        }
        this.cd.detectChanges();
    }

    updateMissingTransactionPurpose(): void {
        this.customerCDD.missingTransactionPurpose = !(this.customerCDD.transactionPurpose || '').trim();
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

    checkCustomerCDD(onCompleted?: () => void): void {
        const customerName = (this.customer.fullName || '').trim();
        if (!customerName || this.checkingCDD || customerName === this.lastCheckedCustomerName) {
            return;
        }

        this.checkingCDD = true;
        this._customerCDDService
            .checkCustomerCDD(new SearchCDDDto({ customerName, createOrEditCustomerCDDDto: this.customerCDD }))
            .subscribe({
                next: (result) => {
                    if (result) {
                        this.customerCDD = result;
                    }
                    this.lastCheckedCustomerName = customerName;
                    this.cd.detectChanges();
                },
                error: (error) => {
                    this.checkingCDD = false;
                    const message = error?.error?.error?.message || error?.error?.message || error?.message || 'Unable to check customer risk profile.';
                    this.notify.error(message);
                    this.cd.detectChanges();
                },
                complete: () => {
                    this.checkingCDD = false;
                    onCompleted?.();
                    this.cd.detectChanges();
                },
            });
    }

        recheckCustomerCDD(): void {
            this.lastCheckedCustomerName = '';
            this.checkCustomerCDD();
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

        const customerName = (this.customer.fullName || '').trim();
        if (this.checkingCDD) {
            return;
        }
        if (customerName && customerName !== this.lastCheckedCustomerName) {
            this.checkCustomerCDD(() => this.save());
            return;
        }

        this.saving = true;
        this.customerCDD.approvedBy = this.appSession.userId;
        const birthDate = this.customer.birthDate as any;
        this.customer.birthDate = birthDate ? (moment.isMoment(birthDate) ? birthDate : moment(birthDate)) : (undefined as any);
        this._lookupService.createCustomer(this.customer).subscribe(
            (newId) => {
                this.customer.id = newId;
                this.customerCDD.customer = newId;
                this._customerCDDService.createOrEdit(this.customerCDD).subscribe(
                    () => {
                        this.notify.info(this.l('SavedSuccessfully'));
                        this.bsModalRef.hide();
                        this.onSave.emit(null);
                        this.warnIfHighRiskCustomer(newId);
                    },
                    (error) => {
                        this.saving = false;
                        const message = error?.error?.error?.message || error?.error?.message || error?.message || 'Unable to save customer risk profile.';
                        this.notify.error(message);
                        this.cd.detectChanges();
                    }
                );
            },
            (error) => {
                this.saving = false;
                const message = error?.error?.error?.message || error?.error?.message || error?.message || 'Unable to save customer.';
                this.notify.error(message);
                this.cd.detectChanges();
            }
        );
    }

    private warnIfHighRiskCustomer(customerId: number | undefined): void {
        if (!customerId) return;
        this._pawnTicketService.isHighRiskCustomer(customerId).subscribe({
            next: (isHighRisk) => {
                if (isHighRisk) {
                    this._modalService.show(HighRiskCustomerWarningDialogComponent, { class: 'modal-md' });
                }
            },
            error: () => undefined,
        });
    }
}