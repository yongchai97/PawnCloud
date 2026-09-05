import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { AppComponentBase } from '@shared/app-component-base';
import { CreateOrEditGoldType, GoldTypeServiceProxy } from '@shared/service-proxies/service-proxies';
import { AbpModalFooterComponent } from '../../../../shared/components/modal/abp-modal-footer.component';
import { AbpModalHeaderComponent } from '../../../../shared/components/modal/abp-modal-header.component';

@Component({
    templateUrl: './edit-gold-type-dialog.component.html',
    standalone: true,
    imports: [CommonModule, FormsModule, InputNumberModule, InputTextModule, AbpModalHeaderComponent, AbpModalFooterComponent],
})
export class EditGoldTypeDialogComponent extends AppComponentBase implements OnInit {
    id!: number;
    saving = false;
    loading = false;
    form = new CreateOrEditGoldType();
    onSave = output<EventEmitter<any>>();

    constructor(
        injector: Injector,
        private goldTypeService: GoldTypeServiceProxy,
        public bsModalRef: BsModalRef,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.loading = true;
        this.goldTypeService.getById(this.id).subscribe(
            (record) => {
                this.form.id = record.id;
                this.form.purity = record.purity;
                this.form.description = record.description;
                this.form.defaultPercentage = record.defaultPercentage;
                this.form.active = record.active;
                this.loading = false;
                this.cd.detectChanges();
            },
            () => {
                this.loading = false;
                this.notify.error('Unable to load gold type.');
                this.bsModalRef.hide();
            }
        );
    }

    save(): void {
        if (!this.form.purity?.trim() || !this.form.description?.trim() || this.form.defaultPercentage < 0) {
            this.notify.warn('Enter valid gold type values.');
            return;
        }

        this.saving = true;
        this.goldTypeService.createOrEdit(this.form).subscribe(
            () => {
                this.notify.info('Gold type saved.');
                this.bsModalRef.hide();
                this.onSave.emit(null);
            },
            () => {
                this.saving = false;
                this.notify.error('Unable to save gold type.');
                this.cd.detectChanges();
            }
        );
    }
}
