import { Component, Injector } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { AbpModalHeaderComponent } from '../../shared/components/modal/abp-modal-header.component';

@Component({
    templateUrl: './high-risk-customer-warning-dialog.component.html',
    standalone: true,
    imports: [AbpModalHeaderComponent],
})
export class HighRiskCustomerWarningDialogComponent extends AppComponentBase {
    constructor(
        injector: Injector,
        public bsModalRef: BsModalRef
    ) {
        super(injector);
    }
}
