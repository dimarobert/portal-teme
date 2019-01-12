export declare interface EditorConfig {
    alignment?: AlignmentConfig;
    autosave?: AutosaveConfig;
    ballonToolbar?: string[] | object;
    blockToolbar?: string[] | object;
    ckfinder?: CKFinderConfig;
    cloudServices?: CloudServicesConfig;
    extraPlugins?: Function[];
    fontFamily?: FontFamilyConfig;
    fontSize?: FontSizeConfig;

    heading?: HeadingConfig;
    highlight?: HighlightConfig;
    image?: ImageConfig;
    language?: string;
    mediaEmbed?: MediaEmbedConfig;
    plugins?: (string | Function)[];
    removePlugins?: string[];
    toolbar?: string[] | object;
    typing?: TypingConfig;
}

export declare interface AlignmentConfig {
    options: ('left' | 'right' | 'center' | 'justify')[];
}

export declare interface AutosaveConfig {
    save: (editor: any) => Promise<any>
}

export declare interface CKFinderConfig {
    openerMethod: string;
    options: object;
    uploadUrl: string;
}

export declare interface CloudServicesConfig {
    documentId: string;
    tokenUrl: string | (() => Promise<any>);
    uploadUrl: string;
    webSocketUrl: string;
}

export declare interface FontFamilyConfig {
    options: (string | FontFamilyOption)[];
}

export declare interface FontFamilyOption {
    model: string;
    title: string;
    upcastAlso: ElementDefinition[];
    view: ElementDefinition;
}

export declare type ElementDefinition = string | object;

export declare interface FontSizeConfig {
    options: (string | number | FontSizeOption)[];
}

export declare interface FontSizeOption {
    model: string;
    title: string;
    upcastAlso: ElementDefinition[];
    view: ElementDefinition;
}

export declare interface HeadingConfig {
    options: HeadingOption[];
}

export declare interface HeadingOption {
    class: string;
    converterPriority: PriorityString;
    icon: string;
    model: string;
    title: string;
    upcastAlso: MatcherPattern | MatcherPattern[];
    view: ElementDefinition;
}

export declare type PriorityString = 'highest' | 'high' | 'normal' | 'low' | 'lowest';
export declare type MatcherPattern = string | RegExp | object | Function;

export declare interface HighlightConfig {
    options: HighlightOption[];
}

export declare interface HighlightOption {
    class: string;
    color: string;
    model: string;
    title: string;
    type: 'marker' | 'pen';
}

export declare interface ImageConfig {
    styles: ImageStyleFormat[];
    toolbar: string[];
}

export declare interface ImageStyleFormat {
    className: string;
    icon: string;
    isDefault: boolean;
    name: string;
    title: string;
}

export declare interface MediaEmbedConfig {
    extraProviders: MediaEmbedProvider[];
    previewsInData: boolean;
    providers: MediaEmbedProvider[];
    removeProviders: string[];
    toolbar: string[];
}

export declare interface MediaEmbedProvider {
    html: Function;
    name: string;
    url: RegExp | RegExp[];
}

export declare interface TypingConfig {
    undoStep: number;
}