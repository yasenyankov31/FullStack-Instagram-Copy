<template >
    <Navbar />
    <div class="container instaFeedBodyWrap">
        <div class="row g-5">
            <div class="col-sm-8">
                <div>
                    <div v-for="data in posts">
                        <Post :postData="data" :userProfilePic="data.userProfilePicture" :isModal="false"
                            :updateMethod="getPosts" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
import Navbar from './Navbar.vue'
import Post from './Post.vue'
import axios from 'axios'
export default {
    components: {
        Navbar,
        Post
    },
    data() {
        return {
            posts: null
        }
    },
    methods: {
        getPosts() {
            axios("https://localhost:7006/api/Posts", {
                method: 'get',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + localStorage.getItem('token')
                }
            }).then(res => {
                this.posts = res.data.posts
            })
        }
    },
    mounted() {
        this.getPosts()

    }

}
</script>
<style scoped>
.profileImage {
    border-radius: 100%;
    max-width: 40px;
}

.profileImageList {
    border-radius: 100%;
    max-width: 60px;
    border: 2px solid #f00;
    padding: 2px;
}

.profileImgSm {
    border-radius: 100%;
    max-width: 20px;
}

.instaLogo {
    max-width: 100px;
}

.navbar {
    position: fixed;
    width: 100%;
    top: 0;
    z-index: 9999;
}

.instaFeedBodyWrap {
    margin-top: 70px;
}
</style>

