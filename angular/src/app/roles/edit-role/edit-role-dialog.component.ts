import { Component, Injector, OnInit, EventEmitter, Output, ChangeDetectorRef } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { forEach as _forEach, includes as _includes, map as _map } from 'lodash-es';
import { NgTemplateOutlet } from '@angular/common';
import { AppComponentBase } from '@shared/app-component-base';
import {
    RoleServiceProxy,
    GetRoleForEditOutput,
    RoleDto,
    PermissionDto,
    RoleEditDto,
    FlatPermissionDto,
} from '@shared/service-proxies/service-proxies';
import { FormsModule } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { TabsetComponent, TabDirective } from 'ngx-bootstrap/tabs';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';

interface PermissionTreeNode<T extends { name: string | undefined }> {
    permission: T;
    children: PermissionTreeNode<T>[];
    depth: number;
}

@Component({
    templateUrl: 'edit-role-dialog.component.html',
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
export class EditRoleDialogComponent extends AppComponentBase implements OnInit {
    @Output() onSave = new EventEmitter<any>();

    saving = false;
    id: number;
    role = new RoleEditDto();
    permissions: FlatPermissionDto[];
    permissionTree: PermissionTreeNode<FlatPermissionDto>[] = [];
    grantedPermissionNames: string[];
    checkedPermissionsMap: { [key: string]: boolean } = {};

    constructor(
        injector: Injector,
        private _roleService: RoleServiceProxy,
        public bsModalRef: BsModalRef,
        private cd: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this._roleService.getRoleForEdit(this.id).subscribe((result: GetRoleForEditOutput) => {
            this.role = result.role;
            this.permissions = result.permissions || [];
            this.permissionTree = this.buildPermissionTree(this.permissions);
            this.grantedPermissionNames = result.grantedPermissionNames;
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
        return checked === undefined ? _includes(this.grantedPermissionNames, permissionName) : checked;
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

    onPermissionChange(permission: FlatPermissionDto, $event) {
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

        const role = new RoleDto();
        role.init(this.role);
        role.grantedPermissions = this.getCheckedPermissions();

        this._roleService.update(role).subscribe(
            () => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.bsModalRef.hide();
                this.onSave.emit();
            },
            () => {
                this.saving = false;
            }
        );
    }
}
