import { Component, Injector, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { GeneralSetup, GeneralSetupServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { InputNumberModule } from 'primeng/inputnumber';
import { DropdownModule } from 'primeng/dropdown';
import { ButtonModule } from 'primeng/button';

@Component({
    selector: 'app-general-setup',
    templateUrl: './general-setup.component.html',
    standalone: true,
    imports: [CommonModule, FormsModule, LocalizePipe, InputNumberModule, DropdownModule, ButtonModule],
})
export class GeneralSetupComponent extends AppComponentBase implements OnInit {
    setup = new GeneralSetup();
    saving = false;
    idMethods = [
        { label: 'Auto Increment Number', value: 1 },
        { label: 'Auto Increment Alphabet', value: 2 },
        { label: 'Random String', value: 3 },
        { label: 'Manual Input', value: 4 },
        { label: 'Pure Random String', value: 5 },
    ];

    constructor(injector: Injector, private setupService: GeneralSetupServiceProxy) {
        super(injector);
        this.setDefaults();
    }

    ngOnInit(): void {
        this.setupService.getGeneralSetup().subscribe((result) => {
            if (result) {
                this.setup = result;
            }
            this.setDefaults();
        });
    }

    save(): void {
        this.saving = true;
        this.setupService.updateGeneralSetup(this.setup).subscribe(
            (result) => {
                this.setup = result || this.setup;
                this.notify.info(this.l('SavedSuccessfully'));
                this.saving = false;
            },
            () => (this.saving = false)
        );
    }

    private setDefaults(): void {
        this.setup.serviceCharge ??= 0.5;
        this.setup.maximumAllowedPercentage ??= 100;
        this.setup.monthsBetweenPledgeAndExpiry ??= 6;
        this.setup.appendYearMonth ??= true;
    }
}
