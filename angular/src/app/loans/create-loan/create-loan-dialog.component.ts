import { Component, Injector, ChangeDetectorRef, ApplicationRef, ViewChild, EventEmitter, output, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { LoanServiceProxy, CreateOrEditLoanDto, MiscFunctionServiceProxy, StatusDto } from '@shared/service-proxies/service-proxies';
import { LookupServiceProxy, PawnTicketLookupDto } from '@shared/service-proxies/lookup-service-proxy';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { ButtonModule } from 'primeng/button';
import { SelectModule, Select } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import { NgFor } from '@angular/common';
import moment from 'moment';

@Component({
    templateUrl: './create-loan-dialog.component.html',
    standalone: true,
    imports: [
        FormsModule,
        AbpModalHeaderComponent,
        AbpValidationSummaryComponent,
        AbpModalFooterComponent,
        LocalizePipe,
        InputTextModule,
        InputNumberModule,
        ButtonModule,
        SelectModule,
        DatePickerModule,
        NgFor,
    ],
})
export class CreateLoanDialogComponent extends AppComponentBase implements OnInit {
    @ViewChild('pawnTicketSelect') pawnTicketSelect: Select;
    @ViewChild('statusSelect') statusSelect: Select;

    saving = false;
    loan = new CreateOrEditLoanDto();
    pawnTickets: PawnTicketLookupDto[] = [];
    statuses: StatusDto[] = [];

    // Local Date fields used to bind p-datepicker (DTO uses moment.Moment)
    interestStartDate: Date | null = null;
    maturityDate: Date | null = null;

    onSave = output<EventEmitter<any>>();

    constructor(
        injector: Injector,
        private _loanService: LoanServiceProxy,
        private _miscService: MiscFunctionServiceProxy,
        private _lookupService: LookupServiceProxy,
        public bsModalRef: BsModalRef,
        public cd: ChangeDetectorRef,
        public appRef: ApplicationRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        if (!this.loan.status) {
            this.loan.status = 'Pending';
        }
        this._lookupService.getPawnTicketsForLookup().subscribe((result) => {
            this.pawnTickets = result.items || [];
            this.refreshSelect(this.pawnTicketSelect);
        });
        this._miscService.getStatuses().subscribe((result) => {
            this.statuses = result.items || [];
            this.refreshSelect(this.statusSelect);
        });
    }

    private refreshSelect(select: Select): void {
        if (!select) return;
        (select as any).cd?.detectChanges?.();
        this.appRef.tick();
    }

    /**
     * PrimeNG's datepicker hides the overlay via setTimeout(150) AFTER
     * onSelect fires. Calling detectChanges() synchronously in onSelect
     * runs while overlayVisible is still true, so the panel stays.
     * Wait past the 150ms timeout before triggering a tick.
     */
    onDateSelectRefresh(): void {
        setTimeout(() => this.appRef.tick(), 200);
    }

    save(): void {
        this.saving = true;
        this.loan.interestStartDate = (this.interestStartDate ? moment(this.interestStartDate) : moment()) as any;
        this.loan.maturityDate = (this.maturityDate ? moment(this.maturityDate) : moment()) as any;
        this._loanService.createOrEdit(this.loan).subscribe(
            () => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.bsModalRef.hide();
                this.onSave.emit(null);
            },
            () => {
                this.saving = false;
                this.appRef.tick();
            }
        );
    }
}
