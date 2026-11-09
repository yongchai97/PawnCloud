import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { forkJoin } from 'rxjs';
import { AppComponentBase } from '@shared/app-component-base';
import {
    CreateOrEditUserOutletDto,
    GeneralSetup,
    GeneralSetupServiceProxy,
    UserOutlet,
    UserOutletServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';

@Component({
    templateUrl: './user-outlet-dialog.component.html',
    styles: [
        `
        .user-outlet-summary {
            background: #f3f8fb;
            border: 1px solid #dceaf0;
            border-radius: .45rem;
        }
        .user-outlet-summary-label {
            color: #71808b;
            font-size: .72rem;
            font-weight: 700;
            letter-spacing: .04em;
            text-transform: uppercase;
        }
        .user-outlet-summary-value {
            color: #1d2b35;
            font-weight: 600;
        }
        .user-outlet-list thead th {
            color: #687782;
            font-size: .72rem;
            text-transform: uppercase;
        }
        .user-outlet-empty {
            border: 1px dashed #ccd9df;
            border-radius: .45rem;
            color: #778792;
        }
        `,
    ],
    standalone: true,
    imports: [CommonModule, FormsModule, AbpModalHeaderComponent, AbpModalFooterComponent, LocalizePipe],
})
export class UserOutletDialogComponent extends AppComponentBase implements OnInit {
    @Output() onSave = new EventEmitter<void>();

    userId: number;
    userName = '';
    mappings: UserOutlet[] = [];
    generalSetups: GeneralSetup[] = [];
    selectedGeneralSetupId: number;
    loading = true;
    saving = false;

    constructor(
        injector: Injector,
        private userOutletService: UserOutletServiceProxy,
        private generalSetupService: GeneralSetupServiceProxy,
        public bsModalRef: BsModalRef,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        forkJoin({
            mappings: this.userOutletService.getAllViaUserId(this.userId),
            generalSetups: this.generalSetupService.getAll(),
        }).subscribe(
            (result) => {
                this.mappings = result.mappings || [];
                this.generalSetups = result.generalSetups || [];
                this.loading = false;
                this.cd.detectChanges();
            },
            (error) => {
                this.loading = false;
                this.notify.error(error?.error?.error?.message || error?.error?.message || 'Unable to load user outlets.');
                this.cd.detectChanges();
            }
        );
    }

    get availableGeneralSetups(): GeneralSetup[] {
        const mappedSetupIds = new Set(this.mappings.map((mapping) => mapping.generalSetup));
        return this.generalSetups.filter((setup) => !mappedSetupIds.has(setup.id));
    }

    getGeneralSetup(mapping: UserOutlet): GeneralSetup | undefined {
        return mapping.generalSetupFk || this.generalSetups.find((setup) => setup.id === mapping.generalSetup);
    }

    save(): void {
        if (!this.userId || !this.selectedGeneralSetupId) {
            this.notify.warn('Select a General Setup option before saving.');
            return;
        }

        const input = new CreateOrEditUserOutletDto();
        input.user = this.userId;
        input.generalSetup = this.selectedGeneralSetupId;
        this.saving = true;

        this.userOutletService.createUserOutlet(input).subscribe(
            () => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.selectedGeneralSetupId = undefined;
                this.loadMappings();
                this.onSave.emit();
            },
            (error) => {
                this.saving = false;
                this.notify.error(error?.error?.error?.message || error?.error?.message || 'Unable to create user outlet.');
                this.cd.detectChanges();
            }
        );
    }

    deleteMapping(mapping: UserOutlet): void {
        const setupName = this.getGeneralSetup(mapping)?.outletName || `General Setup #${mapping.generalSetup}`;
        abp.message.confirm(`Remove ${setupName} from ${this.userName}?`, undefined, (confirmed: boolean) => {
            if (!confirmed) {
                return;
            }

            this.userOutletService.deleteUserOutlet(mapping.id).subscribe(
                () => {
                    this.notify.info(this.l('SuccessfullyDeleted'));
                    this.loadMappings();
                },
                (error) => this.notify.error(error?.error?.error?.message || error?.error?.message || 'Unable to delete user outlet.')
            );
        });
    }

    private loadMappings(): void {
        this.userOutletService.getAllViaUserId(this.userId).subscribe(
            (mappings) => {
                this.mappings = mappings || [];
                this.saving = false;
                this.cd.detectChanges();
            },
            (error) => {
                this.saving = false;
                this.notify.error(error?.error?.error?.message || error?.error?.message || 'Unable to refresh user outlets.');
                this.cd.detectChanges();
            }
        );
    }
}