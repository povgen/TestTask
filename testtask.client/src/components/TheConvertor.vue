<template>
	<div class="d-flex align-items-center flex-column">
		<div class="mt-4">
			<form id="send_files_form">
				<InputFile
					v-model="selectedFiles"
				/>
			</form>
		</div>
		<base-card class="col-lg-7 col-md-8 col-sm-12 mt-4">
			<template #header>
				<div class="d-flex justify-content-between">
					<div>Selected files: <span class="text-danger fs-5">{{ selectedFilesString }}</span></div>

					<button class="btn btn-primary"
							@click="sendFiles"
							:disabled="selectedFiles.length === 0"
							style="min-width: 90px"
					>
						Send files
					</button>
				</div>
			</template>
			<template #body>
				<table class="table table-hover">
					<thead>
					<tr>
						<th>File name</th>
						<th class="text-end">
							<span class="me-5">Options</span>
						</th>
					</tr>
					</thead>
					<tbody>
					<tr v-for="(file, index) in uploadedFiles" :key="index">
						<td>{{ file.name }}</td>

						<td class="text-end">
							<button class="btn btn-success btn_download"
									:disabled="!file.isCompleted"
									@click="getFile(file)"
							>
								Download
							</button>
							<button class="btn btn-danger mx-3"
									:disabled="!file.isCompleted"
									@click="deleteFile(index, $event.target as HTMLButtonElement)"
							>
								Delete
							</button>
						</td>
					</tr>
					</tbody>
				</table>
			</template>
		</base-card>
	</div>
</template>

<script lang="ts">
import {defineComponent} from 'vue'
import BaseCard from './BaseCard.vue'
import InputFile from './InputFile.vue'
import {downloadFile} from '@/helper'

interface FileInfo {
	name: string,
	token: string,
	isCompleted: boolean
}

interface FileItem extends File {
	token: string,
}


export default defineComponent({
	name: 'TheConvertor',
	components: {InputFile, BaseCard},
	data() {
		return {
			selectedFiles: [] as FileItem[],
			uploadedFiles: [] as FileInfo[],

			checkStatusesDelay: 2000
		}
	},
	computed: {
		selectedFilesString() {
			return this.selectedFiles.map(el => el.name).join(', ')
		}
	},
	mounted() {
		const fileList = localStorage.getItem('fileList')
		if (fileList) {
			JSON.parse(fileList).forEach((el: FileInfo) => this.uploadedFiles.push(el))
		}
		this.checkStatus()
	},
	watch: {
		uploadedFiles: {
			handler(newValue) {
				localStorage.setItem('fileList', JSON.stringify(newValue))
			},
			deep: true
		}
	},
	methods: {
		async sendFiles() {

			const formData = new FormData()
			this.selectedFiles.forEach(file => {
					formData.append('files', file)
				}
			)

			const res = await fetch('/api/file', {
				method: 'POST',
				body: formData
			})
			if (!res.ok) {
				alert(await res.text())
				return
			}

			const json = await res.json()
			json['fileTokens'].forEach((fileKey: string, index: number) => {
				const fileInfo: FileInfo = {
					name: this.selectedFiles[index].name,
					token: fileKey,
					isCompleted: false,
				}
				this.uploadedFiles.unshift(fileInfo)
			})
			this.selectedFiles.splice(0);
			setTimeout(this.checkStatus, 750)

		},
		async getFile(file: FileInfo) {
			const res = await fetch(`/api/file?fileToken=${file.token}`)
			downloadFile(await res.blob(), file.name.replace('html', 'pdf'))
		},
		async checkStatus() {
			const index = this.uploadedFiles.findIndex(el => !el.isCompleted)
			console.log(index)
			if (index < 0) {
				return;
			}
			
			const res = await fetch(`/api/file/status?fileToken=${this.uploadedFiles[index].token}`)

			if (!res.ok) {
				console.error(res)
				return
			}
			const json = await res.json()

			if (json.status !== 'completed') {
				setTimeout(this.checkStatus, this.checkStatusesDelay)
				return
			}

			this.uploadedFiles[index].isCompleted = true
			
			setTimeout(this.checkStatus, 10)
		},


		async deleteFile(fileIndex: number, btn: HTMLButtonElement) {
			btn.disabled = true
			const res = await fetch(`/api/file?fileToken=${this.uploadedFiles[fileIndex].token}`, {
				method: 'DELETE'
			})

			if (res.ok) {
				this.uploadedFiles.splice(fileIndex, 1)
			} else {
				console.error(res)
			}
			btn.disabled = false

		},
	}
})
</script>

<style scoped>

</style>