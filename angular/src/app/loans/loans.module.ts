import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { LoansRoutingModule } from './loans-routing.module';
import { LoansComponent } from './loans.component';
import { CreateLoanDialogComponent } from './create-loan/create-loan-dialog.component';
import { EditLoanDialogComponent } from './edit-loan/edit-loan-dialog.component';
import { CommonModule } from '@angular/common';

@NgModule({
    imports: [
        SharedModule,
        LoansRoutingModule,
        CommonModule,
        LoansComponent,
        CreateLoanDialogComponent,
        EditLoanDialogComponent,
    ],
})
export class LoansModule {}