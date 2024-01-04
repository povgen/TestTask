/* eslint-disable */
declare module '*.vue' {
    import type { DefineComponent } from 'vue'
    const component: DefineComponent<{}, {}, any>
    export default component
}

import { Store } from 'vuex'

declare module '@vue/runtime-core' {
    interface State {
        count: number
    }

    interface ComponentCustomProperties {
        $store: Store<State>
    }
}
