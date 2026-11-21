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
import { CustomerServiceProxy, CustomerCDDServiceProxy, AuthorizedPeopleServiceProxy, CreateOrEditCustomerDto, CreateOrEditCustomerCDDDto, SearchCDDDto, CustomerCDD, GetCustomerForEditOutput, BasicCodeServiceProxy, BasicCodeLookupDto, Country, CountryServiceProxy, MiscMasterConfigServiceProxy, MiscMasterConfigLookupDto, CustomerPicture, CustomerOutletDto, CreateOrEditCustomerOutletDto, GeneralSetup, PawnTicketServiceProxy, PawnTicketWithRedeemStatusDto, AuthorizedPeople } from '@shared/service-proxies/service-proxies';
import { LookupServiceProxy } from '@shared/service-proxies/lookup-service-proxy';
import { CustomerPictureServiceProxy } from '@shared/service-proxies/customer-picture-service-proxy';
import { OutletServiceProxy } from '@shared/service-proxies/outlet-service-proxy';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { NgIf, NgFor } from '@angular/common';
import { DropdownModule } from 'primeng/dropdown';
import moment from 'moment';
import { HighRiskCustomerWarningDialogComponent } from '../high-risk-customer-warning-dialog.component';
import { AuthorizedPersonDialogComponent } from '../authorized-person/authorized-person-dialog.component';

@Component({
    templateUrl: './edit-customer-dialog.component.html',
    standalone: true,
    styles: [`
        .form-check {
            min-height: 1.75rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        .form-check-input {
            width: 1.25rem;
            height: 1.25rem;
            margin-top: 0;
            cursor: pointer;
        }

        .form-check-input:disabled {
            cursor: not-allowed;
            opacity: 0.65;
        }
    `],
    imports: [FormsModule, AbpModalHeaderComponent, AbpValidationSummaryComponent, AbpModalFooterComponent, LocalizePipe, InputTextModule, InputNumberModule, ButtonModule, TableModule, NgIf, NgFor, DropdownModule, TabsModule],
})
export class EditCustomerDialogComponent extends AppComponentBase implements OnInit {
    saving = false;
    customer = new CreateOrEditCustomerDto();
    customerCDD = new CreateOrEditCustomerCDDDto();
    checkingCDD = false;
    private lastCheckedCustomerName = '';
    id?: number;
    customerOnly = false;

    // BasicCode dropdowns
    countryOptions: { label: string; value: number }[] = [];
    raceOptions: BasicCodeLookupDto[] = [];
    genderOptions: BasicCodeLookupDto[] = [];
    nationalityOptions: { label: string; value: number }[] = [];
    maritalStatusOptions: BasicCodeLookupDto[] = [];
    paymentModeOptions: BasicCodeLookupDto[] = [];
    deliveryChannelOptions: BasicCodeLookupDto[] = [];
    customerPictures: CustomerPicture[] = [];
    pictureUrls = new Map<number, string>();
    uploadingPicture = false;
    outletOptions: { label: string; value: number }[] = [];
    customerOutlets: CustomerOutletDto[] = [];
    selectedOutlet?: number;
    authorizedPeople: AuthorizedPeople[] = [];
    pawnTickets: PawnTicketWithRedeemStatusDto[] = [];
    loadingPawnTickets = false;

    onSave = output<EventEmitter<any>>();

    constructor(
        injector: Injector,
        private _customerService: CustomerServiceProxy,
        private _customerCDDService: CustomerCDDServiceProxy,
        private _authorizedPeopleService: AuthorizedPeopleServiceProxy,
        private _pawnTicketService: PawnTicketServiceProxy,
        private _modalService: BsModalService,
        private _lookupService: LookupServiceProxy,
        private _basicCodeService: BasicCodeServiceProxy,
        private _countryService: CountryServiceProxy,
        private _miscMasterConfigService: MiscMasterConfigServiceProxy,
        private _customerPictureService: CustomerPictureServiceProxy,
        private _outletService: OutletServiceProxy,
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
        this.loadOutlets();
        this.customerCDD.approvedBy = this.appSession.userId;
        this.customerCDD.pepStatus = false;
        if (!this.id) return;

        this._customerService.getViaIdForEdit(this.id).subscribe((result: GetCustomerForEditOutput) => {
            this.customer = result.customer;
            this.customer.monthlyIncome = this.customer.monthlyIncome ?? 0;
            if (this.customer.birthDate) {
                this.customer.birthDate = moment(this.customer.birthDate).format('YYYY-MM-DD') as any;
            }
            this.readBirthDateFromNric();
            this.updateAge();
            if (!this.customerOnly) {
                this.loadCustomerCDD();
                this.loadCustomerOutlets();
                this.loadAuthorizedPeople();
                this.loadCustomerPictures();
            }
            this.cd.detectChanges();
        }, (error) => {
            const message = error?.error?.error?.message || error?.error?.message || error?.message || 'Unable to load customer for editing.';
            this.notify.error(message);
            this.bsModalRef.hide();
        });
    }

    private loadOutlets(): void {
        this._outletService.getAll().subscribe((outlets: GeneralSetup[]) => {
            this.outletOptions = (outlets || []).filter((outlet) => !!outlet.id).map((outlet) => ({
                label: outlet.outletName || `Outlet ${outlet.id}`,
                value: outlet.id,
            }));
            this.cd.detectChanges();
        });
    }

    private loadCustomerOutlets(): void {
        if (!this.id) return;
        this._customerService.getCustomerOutletViaCustomerId(this.id).subscribe((result) => {
            this.customerOutlets = result || [];
            this.cd.detectChanges();
        });
    }

    private loadAuthorizedPeople(): void {
        if (!this.id) return;
        this._authorizedPeopleService.getByCustomerId(this.id).subscribe(
            (result) => {
                this.authorizedPeople = result ? [result] : [];
                this.cd.detectChanges();
            },
            (error) => this.notify.error(error?.error?.error?.message || error?.error?.message || 'Unable to load authorized people.')
        );
    }

    loadPawnTickets(): void {
        if (!this.id) return;

        this.loadingPawnTickets = true;
        this._pawnTicketService.getAllTicketsForCustomer(this.id).subscribe(
            (result) => {
                this.pawnTickets = result || [];
                this.loadingPawnTickets = false;
                this.cd.detectChanges();
            },
            (error) => {
                this.loadingPawnTickets = false;
                this.notify.error(error?.error?.error?.message || error?.error?.message || 'Unable to load pawn tickets.');
                this.cd.detectChanges();
            }
        );
    }

    isTicketRedeemed(ticket: PawnTicketWithRedeemStatusDto): boolean {
        return !!ticket.createOrEditRedeemRecordDto?.redeemedDate;
    }

    formatTicketDate(value: moment.Moment | undefined): string {
        return value ? moment(value).format('DD/MM/YYYY') : '-';
    }

    addAuthorizedPerson(): void {
        this.openAuthorizedPersonDialog();
    }

    editAuthorizedPerson(person: AuthorizedPeople): void {
        this.openAuthorizedPersonDialog(person);
    }

    private openAuthorizedPersonDialog(person?: AuthorizedPeople): void {
        if (!this.id) return;

        const dialog = this._modalService.show(AuthorizedPersonDialogComponent, {
            class: 'modal-xl',
            initialState: {
                customerId: this.id,
                source: person,
            },
        });
        dialog.content.onSave.subscribe(() => this.loadAuthorizedPeople());
    }

    deleteAuthorizedPerson(person: AuthorizedPeople): void {
        if (!person.id) return;
        abp.message.confirm(`Delete authorized person ${person.fullName || ''}?`, undefined, (confirmed: boolean) => {
            if (!confirmed) return;
            this._authorizedPeopleService.delete(person.id).subscribe(
                () => {
                    this.notify.info(this.l('SuccessfullyDeleted'));
                    this.loadAuthorizedPeople();
                },
                (error) => this.notify.error(error?.error?.error?.message || error?.error?.message || 'Unable to delete authorized person.')
            );
        });
    }

    private loadCustomerCDD(): void {
        if (!this.id) return;
        this._customerCDDService.getViaCustomerId(this.id).subscribe((result: CustomerCDD) => {
            this.customerCDD = result ? new CreateOrEditCustomerCDDDto(result as any) : new CreateOrEditCustomerCDDDto({ customer: this.id } as any);
            this.customerCDD.pepStatus = !!this.customerCDD.pepStatus;
            this.cd.detectChanges();
        });
    }

    addCustomerOutlet(): void {
        if (!this.id || !this.selectedOutlet || this.customerOutlets.some((link) => link.generalSetup === this.selectedOutlet)) return;
        const input = new CreateOrEditCustomerOutletDto({ id: undefined, customer: this.id, generalSetup: this.selectedOutlet });
        this._customerService.createOrEditCustomerOutlet(input).subscribe(() => {
            this.selectedOutlet = undefined;
            this.loadCustomerOutlets();
        });
    }

    removeCustomerOutlet(link: CustomerOutletDto): void {
        if (!link.id) return;
        this._customerService.deleteCustomerOutlet(link.id).subscribe(() => this.loadCustomerOutlets());
    }

    outletName(id: number | undefined): string {
        return this.outletOptions.find((outlet) => outlet.value === id)?.label || `Outlet ${id}`;
    }

    private loadCountries(): void {
        this._countryService.getAllViaYear(new Date().getFullYear()).subscribe((countries: Country[]) => {
            this.countryOptions = (countries || []).map((country) => ({
                label: `${country.countryCodeThreeAlphabet} - ${country.countryName}`,
                value: country.id,
            }));
            this.nationalityOptions = this.countryOptions;
            const malaysia = countries?.find((country) => country.countryCodeThreeAlphabet?.toUpperCase() === 'MYS' || country.countryName?.toLowerCase().includes('malaysia'));
            if (malaysia && !this.customer.nationality) {
                this.customer.nationality = malaysia.id;
            }
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
        if (this.customerOnly) {
            this.lastCheckedCustomerName = customerName;
            onCompleted?.();
            return;
        }
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

    loadCustomerPictures(): void {
        if (!this.id) return;
        this._customerPictureService.getList(this.id).subscribe((pictures) => {
            this.customerPictures = pictures || [];
            this.customerPictures.forEach((picture) => {
                if (picture.id) {
                    this._customerPictureService.download(picture.id).subscribe((blob) => {
                        this.pictureUrls.set(picture.id!, URL.createObjectURL(blob));
                        this.cd.detectChanges();
                    });
                }
            });
            this.cd.detectChanges();
        });
    }

    recheckCustomerCDD(): void {
        this.lastCheckedCustomerName = '';
        this.checkCustomerCDD();
    }

    uploadPicture(event: Event): void {
        const input = event.target as HTMLInputElement;
        const file = input.files?.[0];
        if (!file || !this.id) return;

        this.uploadingPicture = true;
        this._customerPictureService.upload(this.id, file).subscribe({
            next: () => {
                input.value = '';
                this.loadCustomerPictures();
            },
            error: (error) => {
                this.uploadingPicture = false;
                const message = error?.error?.error?.message || error?.error?.message || error?.message || 'Unable to upload file.';
                this.notify.error(message);
                this.cd.detectChanges();
            },
            complete: () => {
                this.uploadingPicture = false;
                this.cd.detectChanges();
            },
        });
    }

    deletePicture(picture: CustomerPicture): void {
        if (!picture.id) return;
        this._customerPictureService.delete(picture.id).subscribe(() => {
            const url = this.pictureUrls.get(picture.id!);
            if (url) URL.revokeObjectURL(url);
            this.pictureUrls.delete(picture.id!);
            this.customerPictures = this.customerPictures.filter((item) => item.id !== picture.id);
            this.cd.detectChanges();
        });
    }

    isImage(picture: CustomerPicture): boolean {
        return !!picture.contentType?.startsWith('image/');
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
        this.customer.id = this.id;
        if (this.customerOnly) {
            this._customerService.createOrEdit(this.customer).subscribe(
                () => {
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
            return;
        }
        this._customerService.createOrEdit(this.customer).subscribe(
            (customerId) => {
                this.customerCDD.customer = customerId;
                this._customerCDDService.createOrEdit(this.customerCDD).subscribe(
                    () => {
                        this.notify.info(this.l('SavedSuccessfully'));
                        this.bsModalRef.hide();
                        this.onSave.emit(null);
                        this.warnIfHighRiskCustomer(customerId);
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