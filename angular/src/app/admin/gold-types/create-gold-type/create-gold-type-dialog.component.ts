import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { AppComponentBase } from '@shared/app-component-base';
import { CreateOrEditGoldType, GoldTypeServiceProxy } from '@shared/service-proxies/service-proxies';
import { AbpModalFooterComponent } from '../../../../shared/components/modal/abp-modal-footer.component';
import { AbpModalHeaderComponent } from '../../../../shared/components/modal/abp-modal-header.component';

@Component({
    templateUrl: './create-gold-type-dialog.component.html',
    standalone: true,
    imports: [FormsModule, ButtonModule, InputNumberModule, InputTextModule, AbpModalHeaderComponent, AbpModalFooterComponent],
})
export class CreateGoldTypeDialogComponent extends AppComponentBase implements OnInit {
    saving = false;
    form = new CreateOrEditGoldType();
    onSave = output<EventEmitter<any>>();

    constructor(
        injector: Injector,
        private goldTypeService: GoldTypeServiceProxy,
        public bsModalRef: BsModalRef,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
        this.form.active = true;
        this.form.defaultPercentage = 0;
    }

    ngOnInit(): void {}

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
