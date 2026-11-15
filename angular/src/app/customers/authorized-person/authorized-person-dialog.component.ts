import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import moment from 'moment';
import {
    AuthorizedPeople,
    AuthorizedPeopleServiceProxy,
    BasicCodeLookupDto,
    Country,
    CountryServiceProxy,
    CreateOrEditAuthorizedPeopleDto,
} from '@shared/service-proxies/service-proxies';
import { LookupServiceProxy } from '@shared/service-proxies/lookup-service-proxy';
import { AppComponentBase } from '@shared/app-component-base';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';

@Component({
    templateUrl: './authorized-person-dialog.component.html',
    standalone: true,
    imports: [FormsModule, AbpModalFooterComponent, AbpModalHeaderComponent, AbpValidationSummaryComponent, DropdownModule, InputTextModule],
})
export class AuthorizedPersonDialogComponent extends AppComponentBase implements OnInit {
    @Output() onSave = new EventEmitter<void>();

    customerId: number;
    source?: AuthorizedPeople;
    person = new CreateOrEditAuthorizedPeopleDto();
    countryOptions: { label: string; value: number }[] = [];
    raceOptions: BasicCodeLookupDto[] = [];
    genderOptions: BasicCodeLookupDto[] = [];
    nationalityOptions: { label: string; value: number }[] = [];
    saving = false;

    constructor(
        injector: Injector,
        private authorizedPeopleService: AuthorizedPeopleServiceProxy,
        private lookupService: LookupServiceProxy,
        private countryService: CountryServiceProxy,
        public bsModalRef: BsModalRef,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        if (this.source) {
            this.person = new CreateOrEditAuthorizedPeopleDto(this.source as any);
            this.person.birthDate = this.source.birthDate ? moment(this.source.birthDate).format('YYYY-MM-DD') as any : (undefined as any);
        }
        this.person.customer = this.customerId;
        this.person.member = !!this.person.member;
        this.person.blacklisted = !!this.person.blacklisted;
        this.person.age = this.person.age || 0;
        this.readBirthDateFromNric();
        this.updateAge();
        this.loadCountries();
        this.loadBasicCodeDropdowns();
    }

    private loadCountries(): void {
        this.countryService.getAllViaYear(new Date().getFullYear()).subscribe((countries: Country[]) => {
            this.countryOptions = (countries || []).map((country) => ({
                label: `${country.countryCodeThreeAlphabet} - ${country.countryName}`,
                value: country.id,
            }));
            this.nationalityOptions = this.countryOptions;
            this.cd.detectChanges();
        });
    }

    private loadBasicCodeDropdowns(): void {
        this.lookupService.getMiscMasterConfigs().subscribe((configs) => {
            const configMap = new Map<string, number>();
            configs.items?.forEach((config) => configMap.set(config.displayName.trim().toLowerCase().replace(/\s+/g, ' '), config.id));
            const race = configMap.get('race');
            const gender = configMap.get('gender');

            if (race) {
                this.lookupService.getBasicCodesByCategory(race).subscribe((result) => {
                    this.raceOptions = result.items || [];
                    this.cd.detectChanges();
                });
            }
            if (gender) {
                this.lookupService.getBasicCodesByCategory(gender).subscribe((result) => {
                    this.genderOptions = result.items || [];
                    this.cd.detectChanges();
                });
            }
        });
    }

    readBirthDateFromNric(): void {
        const digits = (this.person.nric || '').replace(/\D/g, '');
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
            this.person.birthDate = birthDate.format('YYYY-MM-DD') as any;
            this.updateAge();
        }
    }

    updateAge(): void {
        const value = this.person.birthDate as any;
        if (!value) {
            this.person.age = 0;
            return;
        }
        const birthDate = moment.isMoment(value) ? value.toDate() : new Date(`${value}T00:00:00`);
        const age = (Date.now() - birthDate.getTime()) / (365.25 * 24 * 60 * 60 * 1000);
        this.person.age = Number.isFinite(age) && age >= 0 ? Math.floor(age) : 0;
        this.cd.detectChanges();
    }

    save(): void {
        this.person.customer = this.customerId;
        const birthDate = this.person.birthDate as any;
        this.person.birthDate = birthDate ? (moment.isMoment(birthDate) ? birthDate : moment(birthDate)) : (undefined as any);
        this.saving = true;
        this.authorizedPeopleService.createOrEdit(this.person).subscribe(
            () => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.bsModalRef.hide();
                this.onSave.emit();
            },
            (error) => {
                this.saving = false;
                this.notify.error(error?.error?.error?.message || error?.error?.message || 'Unable to save authorized person.');
                this.cd.detectChanges();
            }
        );
    }
}