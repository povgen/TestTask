<template>
	<input 
		id="file_input"
		class="d-none"
		type="file"
		name="files"
		multiple
		accept=".html"
		@input="updateModelValue"
	/>
	<label for="file_input" class="btn btn-primary fs-2 mw-350">
		ADD FILE(S)
	</label>
</template>
<script lang="ts">
import {defineComponent} from 'vue'
import type {PropType} from 'vue'

export default defineComponent({
	name: 'InputFile',
	props: {
		modelValue: {
			type: Array as PropType<File[]>
		}
	},
	methods: {
		updateModelValue(event: Event) {
			const htmlInputElement: HTMLInputElement = event.target as HTMLInputElement
			let files: File[] = []

			if (!htmlInputElement.files) return;

			for (let i = 0; i < htmlInputElement.files.length; i++) {
				const file = htmlInputElement.files.item(i) as File
				if (file.type === 'text/html') {
					files.push(file)
				}
			}

			this.$emit('update:modelValue', files)

		}
	}
})
</script>


<style scoped>

</style>