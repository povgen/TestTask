<template>
	<div class="card">
		<div class="card-header">
			<slot name="header"></slot>
		</div>
		<div class="card-body">
			<slot name="body"></slot>
		</div>
	</div>
</template>

<script lang="ts">
import {defineComponent} from 'vue'

export default defineComponent({
	name: "BaseCard",
	data() {
		return {}
	}
})
</script>
<style scoped>

</style>