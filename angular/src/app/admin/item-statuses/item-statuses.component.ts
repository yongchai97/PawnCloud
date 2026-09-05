import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { finalize } from 'rxjs/operators';
import { TableModule } from 'primeng/table';
import { ItemStatus, ItemStatusServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';

@Component({
    templateUrl: './item-statuses.component.html',
    standalone: true,
    imports: [CommonModule, TableModule],
    animations: [appModuleAnimation()],
})
export class ItemStatusesComponent extends AppComponentBase implements OnInit {
    records: ItemStatus[] = [];
    loading = false;

    constructor(
        injector: Injector,
        private itemStatusService: ItemStatusServiceProxy,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.loadRecords();
    }

    loadRecords(): void {
        this.loading = true;
        this.itemStatusService.getAll().pipe(finalize(() => (this.loading = false))).subscribe((result) => {
            this.records = result || [];
            this.cd.detectChanges();
        });
    }
}
