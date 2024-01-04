﻿export function downloadFile(blob: Blob, name: string) {

    const url = window.URL.createObjectURL(blob)

    const a = document.createElement('a')

    a.style.display = 'none';
    a.href = url;
    a.download = name;

    document.body.appendChild(a)
    a.click();
    document.body.removeChild(a)

    window.URL.revokeObjectURL(url);
}