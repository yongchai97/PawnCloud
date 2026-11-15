import { Component, Injector, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';

@Component({
    templateUrl: './audit-parameters-dialog.component.html',
    standalone: true,
    imports: [AbpModalHeaderComponent],
})
export class AuditParametersDialogComponent extends AppComponentBase implements OnInit {
    parameters?: string;
    formattedParameters = '';

    constructor(
        injector: Injector,
        public bsModalRef: BsModalRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.formattedParameters = this.formatParameters(this.parameters);
    }

    private formatParameters(parameters?: string): string {
        if (!parameters) return '';
        try {
            return JSON.stringify(JSON.parse(parameters), null, 2);
        } catch {
            return parameters;
        }
    }
}