import { NgModule } from '@angular/core';
import { GoldPriceEntryComponent } from './gold-price-entry.component';
import { GoldPriceEntryRoutingModule } from './gold-price-entry-routing.module';

@NgModule({
    imports: [GoldPriceEntryRoutingModule, GoldPriceEntryComponent],
})
export class GoldPriceEntryModule {}