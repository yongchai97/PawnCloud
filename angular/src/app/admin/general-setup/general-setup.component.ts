import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Country, CountryServiceProxy, GeneralSetup, GeneralSetupServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { GeneralSetupDialogComponent } from './general-setup-dialog/general-setup-dialog.component';

@Component({
    selector: 'app-general-setup',
    templateUrl: './general-setup.component.html',
    standalone: true,
    imports: [CommonModule, LocalizePipe],
})
export class GeneralSetupComponent extends AppComponentBase implements OnInit {
    setups: GeneralSetup[] = [];
    countries: Country[] = [];
    countryOptions: { label: string; value: number }[] = [];
    idMethods = [
        { label: 'Auto Increment Number', value: 1 },
        { label: 'Auto Increment Alphabet', value: 2 },
        { label: 'Random String', value: 3 },
        { label: 'Manual Input', value: 4 },
        { label: 'Pure Random String', value: 5 },
    ];
    constructor(
        injector: Injector,
        private setupService: GeneralSetupServiceProxy,
        private countryService: CountryServiceProxy,
        private modalService: BsModalService,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.loadCountries();
        this.refresh();
    }

    private loadCountries(): void {
        this.countryService.getAll().subscribe((countries) => {
            this.countries = countries || [];
            this.countryOptions = this.countries.map((country) => ({
                label: country.countryName || country.countryCodeThreeAlphabet || String(country.id),
                value: country.id,
            }));
            this.cd.detectChanges();
        });
    }

    refresh(): void {
        this.setupService.getAll().subscribe((result) => {
            this.setups = result || [];
            this.cd.detectChanges();
        });
    }

    newOutlet(): void {
        this.openSetupDialog(new GeneralSetup());
    }

    editOutlet(outlet: GeneralSetup): void {
        this.openSetupDialog(outlet);
    }

    private openSetupDialog(source: GeneralSetup): void {
        const dialog: BsModalRef = this.modalService.show(GeneralSetupDialogComponent, {
            class: 'modal-xl',
            initialState: {
                setup: GeneralSetup.fromJS(source),
                countryOptions: this.countryOptions,
                idMethods: this.idMethods,
            },
        });

        dialog.content.onSave.subscribe(() => this.refresh());
    }

    delete(outlet: GeneralSetup): void {
        if (!outlet.id || !confirm(`Delete ${outlet.outletName || 'this outlet'}?`)) return;
        this.setupService.delete(outlet.id).subscribe(() => {
            this.notify.info(this.l('SuccessfullyDeleted'));
            this.refresh();
            this.cd.detectChanges();
        });
    }

}
