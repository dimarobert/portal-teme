import { Component, OnInit, ViewChild, ElementRef, AfterViewInit, Input } from '@angular/core';
import { HttpClient, HttpEvent, HttpEventType, HttpRequest } from '@angular/common/http';
import { map, last, tap, take, first } from 'rxjs/operators';
import { UploadedFile } from './upload-file.model';
import { BehaviorSubject, Observable } from 'rxjs';

@Component({
  selector: 'app-dropzone-file-upload',
  templateUrl: './dropzone-file-upload.component.html',
  styleUrls: ['./dropzone-file-upload.component.scss']
})
export class DropzoneFileUploadComponent implements OnInit, AfterViewInit {

  @Input() uploadUrl: string;

  constructor(private http: HttpClient) { }

  protected files: FileData[] = [];
  hovered: boolean;
  uploading: boolean;
  progress: BehaviorSubject<number> = new BehaviorSubject(0);
  total: number = 0;

  @ViewChild('fileInput') fileInput: ElementRef<HTMLInputElement>;

  onDragStart($event: DragEvent) {
    this.noPropagation($event);
    this.hovered = true;
  }

  onDragEnd($event: DragEvent) {
    this.hovered = false;
  }

  onDragOver($event: DragEvent) {
    this.noPropagation($event);
    // Makes it possible to drag files from chrome's download bar
    const effect = $event.dataTransfer.effectAllowed;
    $event.dataTransfer.dropEffect = (effect === 'move' || effect === 'linkMove') ? 'move' : 'copy';
  }

  onDrop($event: DragEvent) {
    this.noPropagation($event);

    this.addFiles($event.dataTransfer.files);
    this.hovered = false;
  }

  onClick($event: MouseEvent) {
    const target = <HTMLElement>$event.target;
    if (!target.closest('.file-container'))
      this.fileInput.nativeElement.click();
  }

  ngOnInit() {
  }

  ngAfterViewInit(): void {

  }

  private noPropagation(event: Event) {
    event.stopPropagation();
    if (event.preventDefault) {
      event.preventDefault();
    } else {
      event.returnValue = false;
    }
  }

  onFileInputChange($event: Event) {
    const target = <HTMLInputElement>$event.target;
    this.addFiles(target.files);
    target.value = "";
  }

  addFiles(files: FileList) {
    if (files.length) {
      for (let file of <any>files) {
        const newFileData = {
          file: file,
          dataURL: ''
        };
        this.files.push(newFileData);
        this.createThumbnail(newFileData);
      }
    }
  }

  removeFile(file: FileData) {
    const idx = this.files.indexOf(file);
    this.files.splice(idx, 1);
  }

  getFileSize(file: FileData): string {
    return this.getFriendlySize(file.file.size);
  }

  getFileName(file: FileData): string {
    let fileName = file.file.name;
    let fileExt = this.getExtension(file.file);
    if (fileExt) {
      fileExt = '.' + fileExt;
      fileName = fileName.slice(0, fileName.length - fileExt.length);
    }
    if (fileName.length > 12) {
      fileName = fileName.slice(0, 7) + '&hellip;' + fileName.slice(fileName.length - 5);
    }

    return fileName + fileExt;
  }

  getExtension(file: File): string {
    const matches = this.getRegexMatches(/.([^\.]+)$/, file.name);
    if (matches.length == 0)
      return '';

    return matches[0][0].toLowerCase();
  }

  isImage(file: FileData): boolean {
    const ext = this.getExtension(file.file);

    const imgExtensions = ['jpg', 'jpeg', 'png'];
    if (imgExtensions.indexOf(ext) >= 0)
      return true;

    return false;
  }

  getDocumentIcon(file: FileData): string {
    const ext = this.getExtension(file.file);

    switch (ext) {
      case 'pdf':
        return '';

      case 'xls':
      case 'xlsx':
        return '';

      case 'doc':
      case 'docx':
        return '';

    }

    return '';
  }

  createThumbnail(file: FileData): void {
    if (file.dataURL)
      return;

    file.dataURL = '';
    const fileReader = new FileReader();
    fileReader.onload = () => {
      file.dataURL = <string>fileReader.result;
    };
    fileReader.readAsDataURL(file.file);
  }

  uploadFiles(): Promise<UploadedFile[]> {
    if (!this.uploadUrl)
      throw new Error('No upload url specified. Cannot upload files.');

    const formData = new FormData();
    for (const file of this.files) {
      formData.append(file.file.name, file.file, file.file.name);
    }
    this.uploading = true;
    const uploadRequest = new HttpRequest('POST', this.uploadUrl, formData, {
      reportProgress: true
    })
    return this.http.request<UploadedFile[]>(uploadRequest)
      .pipe(
        tap((event: HttpEvent<UploadedFile[]>) => {
          switch (event.type) {
            case HttpEventType.UploadProgress:
              this.progress.next(event.loaded);
              this.total = event.total;
              break;
          }
        }),
        first(event => event.type === HttpEventType.Response),
        map(event => {
          if (event.type === HttpEventType.Response)
            return event.body;
          return [];
        }),
        take(1)
      ).toPromise();
  }

  computeProgress(): Observable<number> {
    return this.progress
      .pipe(map(currentValue => {
        if (currentValue == 0 || this.total == 0)
          return 0;

        return (currentValue / this.total) * 100;
      }));
  }

  getFriendlySize(size: number) {
    const units = ['B', 'KB', 'MB', 'GB', 'TB'];
    let unit = 0;
    while (size >= 1024) {
      size /= 1024.0;
      unit++;
    }

    let sizeStr = '';
    if (unit > 1)
      sizeStr = size.toFixed(2);
    else
      sizeStr = Math.floor(size).toString();

    return `${sizeStr} ${units[unit]}`;
  }

  getRegexMatches(regex: RegExp, input: string): string[][] {
    if (!(regex instanceof RegExp))
      throw new Error('The provided regex parameter is not of type RegExp.');

    if (!regex.global) {
      // If global flag not set, create new one.
      var flags = "g";
      if (regex.ignoreCase) flags += "i";
      if (regex.multiline) flags += "m";
      if (regex.sticky) flags += "y";
      regex = RegExp(regex.source, flags);
    }

    var matches = [];
    var match = regex.exec(input);
    while (match) {
      if (match.length == 1) // no capturing groups in the regex
        matches.push([match[0]]);
      else
        matches.push(match.slice(1));
      match = regex.exec(input);
    }
    return matches;
  }
}

interface FileData {
  file: File;
  dataURL: string;
}
