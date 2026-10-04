import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import moment from 'moment';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { GeneralSetupServiceProxy, GeneralSetupTable } from '@shared/service-proxies/service-proxies';
import { AbpModalFooterComponent } from '../../../../shared/components/modal/abp-modal-footer.component';
import { AbpModalHeaderComponent } from '../../../../shared/components/modal/abp-modal-header.component';

@Component({
    templateUrl: './general-setup-table-dialog.component.html',
    standalone: true,
    imports: [CommonModule, FormsModule, AbpModalHeaderComponent, AbpModalFooterComponent],
})
export class GeneralSetupTableDialogComponent extends AppComponentBase implements OnInit {
    setupTable = new GeneralSetupTable();
    saving = false;
    onSave = new EventEmitter<void>();
    monthRateProperties: (keyof GeneralSetupTable)[] = [
        'firstMonthInterestRate',
        'secondMonthInterestRate',
        'thirdMonthInterestRate',
        'fourthMonthInterestRate',
        'fifthMonthInterestRate',
        'sixthMonthInterestRate',
        'seventhMonthInterestRate',
        'eighthMonthInterestRate',
        'ninthMonthInterestRate',
        'tenthMonthInterestRate',
        'eleventhMonthInterestRate',
        'twelfthMonthInterestRate',
    ];

    constructor(
        injector: Injector,
        private setupService: GeneralSetupServiceProxy,
        public bsModalRef: BsModalRef,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.setupTable.maximumPercentage ??= 0;
        this.monthRateProperties.forEach((property) => {
            (this.setupTable as any)[property] ??= 0;
        });
    }

    save(): void {
        this.saving = true;
        this.setupService.createOrEditGeneralSetupTable(this.setupTable).subscribe(
            () => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.bsModalRef.hide();
                this.onSave.emit();
            },
            (error) => {
                this.saving = false;
                this.notify.error(error?.error?.error?.message || error?.error?.message || 'Unable to save rate table.');
                this.cd.detectChanges();
            }
        );
    }

    dateValue(value: moment.Moment | undefined): string {
        return value?.isValid() ? value.format('YYYY-MM-DD') : '';
    }

    setDate(value: string): void {
        this.setupTable.effectiveDate = value ? moment(value) : undefined as any;
    }
}
