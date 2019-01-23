import { Type } from '@angular/core';

export class NavLink implements NavLinkOptions {

    constructor(private options: NavLinkOptions) { }

    get label(): string {
        return this.options.label;
    }

    get commands(): any[] {
        return this.options.commands;
    }

    get relative(): boolean {
        return this.options.relativeTo != null;
    }

    get relativeTo(): Type<any> {
        return this.options.relativeTo;
    }

    get exact(): boolean {
        return this.options.exact || false;
    }

    get action(): () => void {
        return this.options.action || (() => { });
    }

    get icon(): string {
        return this.options.icon;
    }
}

export interface NavLinkOptions {
    label: string;
    commands: any[];

    icon?: string;
    relativeTo?: Type<any>;
    exact?: boolean;
    action?: () => void;
}