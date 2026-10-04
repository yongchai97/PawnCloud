import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import moment from 'moment';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { DropdownModule } from 'primeng/dropdown';
import { AppComponentBase } from '@shared/app-component-base';
import { GeneralSetup, GeneralSetupServiceProxy, GeneralSetupTable } from '@shared/service-proxies/service-proxies';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { AbpModalFooterComponent } from '../../../../shared/components/modal/abp-modal-footer.component';
import { AbpModalHeaderComponent } from '../../../../shared/components/modal/abp-modal-header.component';
import { GeneralSetupTableDialogComponent } from '../general-setup-table-dialog/general-setup-table-dialog.component';

@Component({
    templateUrl: './general-setup-dialog.component.html',
    standalone: true,
    imports: [CommonModule, FormsModule, DropdownModule, LocalizePipe, AbpModalHeaderComponent, AbpModalFooterComponent],
})
export class GeneralSetupDialogComponent extends AppComponentBase implements OnInit {
    setup = new GeneralSetup();
    setupTables: GeneralSetupTable[] = [];
    countryOptions: { label: string; value: number }[] = [];
    idMethods = [
        { label: 'Auto Increment Number', value: 1 },
        { label: 'Auto Increment Alphabet', value: 2 },
        { label: 'Random String', value: 3 },
        { label: 'Manual Input', value: 4 },
        { label: 'Pure Random String', value: 5 },
    ];
    saving = false;
    onSave = new EventEmitter<GeneralSetup>();

    constructor(
        injector: Injector,
        private setupService: GeneralSetupServiceProxy,
        private modalService: BsModalService,
        public bsModalRef: BsModalRef,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.setup.serviceCharge ??= 0.5;
        this.setup.maximumAllowedPercentage ??= 100;
        this.setup.monthsBetweenPledgeAndExpiry ??= 6;
        this.setup.appendYearMonth ??= true;
        this.loadSetupTables();
    }

    loadSetupTables(): void {
        if (!this.setup.id) {
            this.setupTables = [];
            return;
        }

        this.setupService.getGeneralSetupTable(this.setup.id).subscribe(
            (result) => {
                this.setupTables = (result || []).sort((left, right) => {
                    return (left.effectiveDate?.valueOf() || 0) - (right.effectiveDate?.valueOf() || 0);
                });
                this.cd.detectChanges();
            },
            (error) => this.notify.error(error?.error?.error?.message || error?.error?.message || 'Unable to load rate tables.')
        );
    }

    newSetupTable(): void {
        const setupTable = new GeneralSetupTable();
        setupTable.generalSetup = this.setup.id;
        setupTable.effectiveDate = moment();
        this.openSetupTableDialog(setupTable);
    }

    editSetupTable(table: GeneralSetupTable): void {
        this.openSetupTableDialog(table);
    }

    private openSetupTableDialog(source: GeneralSetupTable): void {
        const dialog: BsModalRef = this.modalService.show(GeneralSetupTableDialogComponent, {
            class: 'modal-xl',
            initialState: {
                setupTable: GeneralSetupTable.fromJS(source),
            },
        });

        dialog.content.onSave.subscribe(() => this.loadSetupTables());
    }

    deleteSetupTable(table: GeneralSetupTable): void {
        if (!table.id || !confirm(`Delete the rate table for ${this.dateValue(table.effectiveDate)}?`)) return;

        this.setupService.deleteGeneralSetupTable(table.id).subscribe(
            () => {
                this.notify.info(this.l('SuccessfullyDeleted'));
                this.loadSetupTables();
            },
            (error) => this.notify.error(error?.error?.error?.message || error?.error?.message || 'Unable to delete rate table.')
        );
    }

    save(): void {
        this.saving = true;
        this.setupService.createOrEdit(this.setup).subscribe(
            () => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.bsModalRef.hide();
                this.onSave.emit(this.setup);
            },
            (error) => {
                this.saving = false;
                this.notify.error(error?.error?.error?.message || error?.error?.message || 'Unable to save outlet.');
                this.cd.detectChanges();
            }
        );
    }

    dateValue(value: moment.Moment | undefined): string {
        return value?.isValid() ? value.format('YYYY-MM-DD') : '';
    }

    setDate(field: keyof GeneralSetup, value: string): void {
        (this.setup as any)[field] = value ? moment(value) : undefined;
    }
}
