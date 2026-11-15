import { Component, Injector, OnInit, EventEmitter, output, ChangeDetectorRef } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import {
    RoleServiceProxy,
    RoleDto,
    PermissionDto,
    CreateRoleDto,
    PermissionDtoListResultDto,
} from '@shared/service-proxies/service-proxies';
import { forEach as _forEach, map as _map } from 'lodash-es';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { TabsetComponent, TabDirective } from 'ngx-bootstrap/tabs';
import { NgTemplateOutlet } from '@angular/common';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';

interface PermissionTreeNode<T extends { name: string | undefined }> {
    permission: T;
    children: PermissionTreeNode<T>[];
    depth: number;
}

@Component({
    templateUrl: 'create-role-dialog.component.html',
    standalone: true,
    styles: [`
        .permission-tree {
            list-style: none;
            margin: 0;
            padding: 0;
        }

        .permission-tree .permission-tree {
            border-left: 1px solid #dee2e6;
            margin-left: 0.75rem;
            padding-left: 1.25rem;
        }

        .permission-tree li {
            position: relative;
            padding: 0.35rem 0;
        }

        .permission-tree .permission-tree > li::before {
            border-top: 1px solid #dee2e6;
            content: '';
            left: -1.25rem;
            position: absolute;
            top: 1rem;
            width: 1rem;
        }
    `],
    imports: [
        FormsModule,
        AbpModalHeaderComponent,
        TabsetComponent,
        TabDirective,
        NgTemplateOutlet,
        AbpValidationSummaryComponent,
        AbpModalFooterComponent,
        LocalizePipe,
    ],
})
export class CreateRoleDialogComponent extends AppComponentBase implements OnInit {
    saving = false;
    role = new RoleDto();
    permissions: PermissionDto[] = [];
    permissionTree: PermissionTreeNode<PermissionDto>[] = [];
    checkedPermissionsMap: { [key: string]: boolean } = {};
    defaultPermissionCheckedStatus = true;

    onSave = output<EventEmitter<any>>();

    constructor(
        injector: Injector,
        private _roleService: RoleServiceProxy,
        public bsModalRef: BsModalRef,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this._roleService.getAllPermissions().subscribe((result: PermissionDtoListResultDto) => {
            this.permissions = result.items || [];
            this.permissionTree = this.buildPermissionTree(this.permissions);
            this.setInitialPermissionsStatus();
            this.cd.detectChanges();
        });
    }

    setInitialPermissionsStatus(): void {
        _map(this.permissions, (item) => {
            this.checkedPermissionsMap[item.name] = this.isPermissionChecked(item.name);
        });
    }

    isPermissionChecked(permissionName: string): boolean {
        const checked = this.checkedPermissionsMap[permissionName];
        return checked === undefined ? this.defaultPermissionCheckedStatus : checked;
    }

    private buildPermissionTree<T extends { name: string | undefined }>(permissions: T[]): PermissionTreeNode<T>[] {
        const nodes = new Map<string, PermissionTreeNode<T>>();
        const roots: PermissionTreeNode<T>[] = [];

        permissions.forEach((permission) => {
            if (permission.name) {
                nodes.set(permission.name, { permission, children: [], depth: 0 });
            }
        });

        permissions.forEach((permission) => {
            if (!permission.name) return;

            const node = nodes.get(permission.name);
            if (!node) return;

            const separatorIndex = permission.name.lastIndexOf('.');
            const parentName = separatorIndex > 0 ? permission.name.substring(0, separatorIndex) : undefined;
            const parent = parentName ? nodes.get(parentName) : undefined;

            if (parent) {
                node.depth = parent.depth + 1;
                parent.children.push(node);
            } else {
                roots.push(node);
            }
        });

        return roots;
    }

    onPermissionChange(permission: PermissionDto, $event) {
        if (!permission.name) return;

        const checked = $event.target.checked;
        const descendantPrefix = `${permission.name}.`;
        this.permissions.forEach((item) => {
            if (item.name === permission.name || item.name?.startsWith(descendantPrefix)) {
                this.checkedPermissionsMap[item.name] = checked;
            }
        });
    }

    getCheckedPermissions(): string[] {
        const permissions: string[] = [];
        _forEach(this.checkedPermissionsMap, function (value, key) {
            if (value) {
                permissions.push(key);
            }
        });
        return permissions;
    }

    save(): void {
        this.saving = true;

        const role = new CreateRoleDto();
        role.init(this.role);
        role.grantedPermissions = this.getCheckedPermissions();

        this._roleService.create(role).subscribe(
            () => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.bsModalRef.hide();
                this.onSave.emit(null);
            },
            () => {
                this.saving = false;
                this.cd.detectChanges();
            }
        );
    }
}
