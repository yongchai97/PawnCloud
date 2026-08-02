import { Component, Injector, ChangeDetectorRef, EventEmitter, output, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CustomerServiceProxy, CreateOrEditCustomerDto, GetCustomerForEditOutput, CustomerDocumentServiceProxy, CreateOrEditCustomerDocumentDto } from '@shared/service-proxies/service-proxies';
import { LookupServiceProxy, CustomerDocumentSummaryDto } from '@shared/service-proxies/lookup-service-proxy';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { NgIf } from '@angular/common';

@Component({
    templateUrl: './edit-customer-dialog.component.html',
    standalone: true,
    imports: [FormsModule, AbpModalHeaderComponent, AbpValidationSummaryComponent, AbpModalFooterComponent, LocalizePipe, InputTextModule, InputNumberModule, ButtonModule, TableModule, NgIf],
})
export class EditCustomerDialogComponent extends AppComponentBase implements OnInit {
    saving = false;
    customer = new CreateOrEditCustomerDto();
    documents: CustomerDocumentSummaryDto[] = [];
    id?: number;

    documentType = '';
    selectedFile: File | null = null;
    uploading = false;

    onSave = output<EventEmitter<any>>();

    constructor(
        injector: Injector,
        private _customerService: CustomerServiceProxy,
        private _customerDocumentService: CustomerDocumentServiceProxy,
        private _lookupService: LookupServiceProxy,
        public bsModalRef: BsModalRef,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        if (this.id) {
            this._customerService.getViaIdForEdit(this.id).subscribe((result: GetCustomerForEditOutput) => {
                this.customer = result.customer;
                this.loadDocuments();
                this.cd.detectChanges();
            });
        }
    }

    loadDocuments(): void {
        if (!this.id) return;
        this._lookupService.getCustomerDocumentsByCustomerId(this.id).subscribe((result) => {
            this.documents = result.items || [];
            this.cd.detectChanges();
        });
    }

    onFileSelected(event: Event): void {
        const input = event.target as HTMLInputElement;
        if (input.files && input.files.length > 0) {
            this.selectedFile = input.files[0];
            if (!this.documentType && this.selectedFile) {
                const ext = this.selectedFile.name.split('.').pop()?.toLowerCase();
                this.documentType = ext ? ext.toUpperCase() : '';
            }
        } else {
            this.selectedFile = null;
        }
    }

    private fileToBase64(file: File): Promise<string> {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.onload = () => {
                const result = reader.result as string;
                const idx = result.indexOf(',');
                resolve(idx >= 0 ? result.substring(idx + 1) : result);
            };
            reader.onerror = () => reject(reader.error);
            reader.readAsDataURL(file);
        });
    }

    deleteDocument(doc: CustomerDocumentSummaryDto): void {
        abp.message.confirm(this.l('DeleteDocumentConfirmation', doc.fileName), undefined, (result: boolean) => {
            if (result && doc.id) {
                this._customerDocumentService.delete(doc.id).subscribe(() => {
                    abp.notify.success(this.l('SuccessfullyDeleted'));
                    this.loadDocuments();
                });
            }
        });
    }

    save(): void {
        this.saving = true;
        this.customer.id = this.id;
        this._customerService.createOrEdit(this.customer).subscribe(
            () => {
                this.uploadDocumentIfNeeded();
            },
            () => {
                this.saving = false;
                this.cd.detectChanges();
            }
        );
    }

    private uploadDocumentIfNeeded(): void {
        if (!this.selectedFile) {
            this.finish();
            return;
        }
        this.uploading = true;
        this.fileToBase64(this.selectedFile).then((b64) => {
            const dto = new CreateOrEditCustomerDocumentDto();
            dto.customer = this.id!;
            dto.documentType = this.documentType || '';
            dto.fileName = this.selectedFile!.name;
            dto.documentContents = b64;
            this._lookupService.createCustomerDocument(dto).subscribe(
                () => this.finish(),
                () => {
                    this.uploading = false;
                    this.saving = false;
                    this.cd.detectChanges();
                }
            );
        });
    }

    private finish(): void {
        this.notify.info(this.l('SavedSuccessfully'));
        this.bsModalRef.hide();
        this.onSave.emit(null);
    }
}