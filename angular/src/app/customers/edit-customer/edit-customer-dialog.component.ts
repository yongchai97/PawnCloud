import { Component, Injector, ChangeDetectorRef, EventEmitter, output, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CustomerServiceProxy, CreateOrEditCustomerDto, GetCustomerForEditOutput, BasicCodeServiceProxy, BasicCodeLookupDto, Country, CountryServiceProxy, MiscMasterConfigServiceProxy, MiscMasterConfigLookupDto, CustomerPicture, CustomerOutletDto, CreateOrEditCustomerOutletDto, GeneralSetup } from '@shared/service-proxies/service-proxies';
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
    countryOptions: { label: string; value: number }[] = [];
    raceOptions: BasicCodeLookupDto[] = [];
    genderOptions: BasicCodeLookupDto[] = [];
    nationalityOptions: { label: string; value: number }[] = [];
    businessNatureOptions: BasicCodeLookupDto[] = [];
    maritalStatusOptions: BasicCodeLookupDto[] = [];
    customerPictures: CustomerPicture[] = [];
    pictureUrls = new Map<number, string>();
    uploadingPicture = false;
    outletOptions: { label: string; value: number }[] = [];
    customerOutlets: CustomerOutletDto[] = [];
    selectedOutlet?: number;

    onSave = output<EventEmitter<any>>();

    constructor(
        injector: Injector,
        private _customerService: CustomerServiceProxy,
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
    }

    ngOnInit(): void {
        this.loadCountries();
        this.loadBasicCodeDropdowns();
        this.loadOutlets();
        if (this.id) {
            this._customerService.getViaIdForEdit(this.id).subscribe((result: GetCustomerForEditOutput) => {
                this.customer = result.customer;
                if (this.customer.birthDate) {
                    this.customer.birthDate = moment(this.customer.birthDate).format('YYYY-MM-DD') as any;
                }
                this.updateAge();
                this.loadCustomerOutlets();
                this.loadCustomerPictures();
                this.cd.detectChanges();
            });
        }
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

    save(): void {
        this.saving = true;
        const birthDate = this.customer.birthDate as any;
        this.customer.birthDate = birthDate ? (moment.isMoment(birthDate) ? birthDate : moment(birthDate)) : (undefined as any);
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