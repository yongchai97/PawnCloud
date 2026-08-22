import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { GoldPriceEntryComponent } from './gold-price-entry.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: GoldPriceEntryComponent,
                data: { title: 'Gold Price Entry' },
            },
        ]),
    ],
})
export class GoldPriceEntryRoutingModule {}