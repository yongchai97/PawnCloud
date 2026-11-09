import { ChangeDetectorRef, Component, Injector, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { TableModule } from 'primeng/table';
import { LazyLoadEvent, PrimeTemplate } from 'primeng/api';
import { Paginator, PaginatorModule } from 'primeng/paginator';
import { ButtonModule } from 'primeng/button';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AuditLogDto, AuditLogServiceProxy } from '@shared/service-proxies/audit-log-service-proxy';

@Component({
    templateUrl: './audit-logs.component.html',
    styleUrls: ['./audit-logs.component.css'],
    standalone: true,
    imports: [CommonModule, FormsModule, TableModule, PrimeTemplate, PaginatorModule, ButtonModule, LocalizePipe],
    animations: [appModuleAnimation()],
})
export class AuditLogsComponent extends PagedListingComponentBase<AuditLogDto> {
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    filter = '';

    get exceptionCount(): number {
        return this.primengTableHelper.records.filter((record) => !!record.exceptionMessage).length;
    }

    constructor(
        injector: Injector,
        private auditLogService: AuditLogServiceProxy,
        cd: ChangeDetectorRef
    ) {
        super(injector, cd);
        this.primengTableHelper.records = [];
    }

    list(event?: LazyLoadEvent): void {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);
            if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
                return;
            }
        }

        this.primengTableHelper.showLoadingIndicator();
        this.auditLogService
            .getAll(
                this.filter,
                this.primengTableHelper.getSkipCount(this.paginator, event),
                this.primengTableHelper.getMaxResultCount(this.paginator, event)
            )
            .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
            .subscribe({
                next: (result) => {
                    this.primengTableHelper.records = result.items || [];
                    this.primengTableHelper.totalRecordsCount = result.totalCount;
                    this.cd.detectChanges();
                },
                error: (error) => {
                    this.notify.error(error?.error?.error?.message || error?.message || 'Unable to load audit logs.');
                    this.cd.detectChanges();
                },
            });
    }

    protected delete(_record: AuditLogDto): void {}
}
