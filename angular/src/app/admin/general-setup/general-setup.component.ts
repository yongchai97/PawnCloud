import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { GeneralSetup } from '@shared/service-proxies/service-proxies';
import { OutletServiceProxy } from '@shared/service-proxies/outlet-service-proxy';
import { AppComponentBase } from '@shared/app-component-base';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { DropdownModule } from 'primeng/dropdown';

@Component({
    selector: 'app-general-setup',
    templateUrl: './general-setup.component.html',
    standalone: true,
    imports: [CommonModule, FormsModule, LocalizePipe, DropdownModule],
})
export class GeneralSetupComponent extends AppComponentBase implements OnInit {
    setups: GeneralSetup[] = [];
    setup = new GeneralSetup();
    editing = false;
    saving = false;
    idMethods = [
        { label: 'Auto Increment Number', value: 1 },
        { label: 'Auto Increment Alphabet', value: 2 },
        { label: 'Random String', value: 3 },
        { label: 'Manual Input', value: 4 },
        { label: 'Pure Random String', value: 5 },
    ];

    constructor(injector: Injector, private setupService: OutletServiceProxy, private cd: ChangeDetectorRef) {
        super(injector);
        this.setDefaults();
    }

    ngOnInit(): void {
        this.refresh();
    }

    refresh(): void {
        this.setupService.getAll().subscribe((result) => {
            this.setups = result || [];
            this.cd.detectChanges();
        });
    }

    newOutlet(): void {
        this.setup = new GeneralSetup();
        this.setDefaults();
        this.editing = true;
    }

    editOutlet(outlet: GeneralSetup): void {
        this.setup = GeneralSetup.fromJS(outlet);
        this.editing = true;
    }

    cancel(): void {
        this.editing = false;
    }

    save(): void {
        this.saving = true;
        this.setupService.createOrEdit(this.setup).subscribe(
            (result) => {
                this.setup = result;
                this.notify.info(this.l('SavedSuccessfully'));
                this.saving = false;
                this.editing = false;
                this.refresh();
                this.cd.detectChanges();
            },
            (error) => {
                this.saving = false;
                this.notify.error(error?.error?.error?.message || error?.error?.message || 'Unable to save outlet.');
                this.cd.detectChanges();
            }
        );
    }

    delete(outlet: GeneralSetup): void {
        if (!outlet.id || !confirm(`Delete ${outlet.outletName || 'this outlet'}?`)) return;
        this.setupService.delete(outlet.id).subscribe(() => {
            this.notify.info(this.l('SuccessfullyDeleted'));
            this.refresh();
            this.cd.detectChanges();
        });
    }

    private setDefaults(): void {
        this.setup.serviceCharge ??= 0.5;
        this.setup.maximumAllowedPercentage ??= 100;
        this.setup.monthsBetweenPledgeAndExpiry ??= 6;
        this.setup.appendYearMonth ??= true;
    }
}
