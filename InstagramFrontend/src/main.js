import { createApp } from 'vue'
import {FontAwesomeIcon} from '@fortawesome/vue-fontawesome'
import { faHeart } from '@fortawesome/free-regular-svg-icons';
import {library} from '@fortawesome/fontawesome-svg-core'
import {fas} from '@fortawesome/free-solid-svg-icons'
import 'bootstrap/dist/css/bootstrap.css'
import App from './App.vue'
import router from './router'
import CKEditor from '@ckeditor/ckeditor5-vue';

library.add(fas)
library.add(faHeart)

createApp(App).use(router).use(CKEditor).component('fa',FontAwesomeIcon).mount('#app')

import 'bootstrap/dist/js/bootstrap'
