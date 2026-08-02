import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { CustomersRoutingModule } from './customers-routing.module';
import { CustomersComponent } from './customers.component';
import { CreateCustomerDialogComponent } from './create-customer/create-customer-dialog.component';
import { EditCustomerDialogComponent } from './edit-customer/edit-customer-dialog.component';
import { CommonModule } from '@angular/common';

@NgModule({
    imports: [
        SharedModule,
        CustomersRoutingModule,
        CommonModule,
        CustomersComponent,
        CreateCustomerDialogComponent,
        EditCustomerDialogComponent,
    ],
})
export class CustomersModule {}
