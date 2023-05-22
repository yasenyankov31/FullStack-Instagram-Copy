<template>
    <Navbar />
    <div class="pageContainer">
        <div class="container">

            <div class="profile">

                <div class="profile-image">

                    <img :src="'https://localhost:7006' + userData.userProfilePicture" style="width: 300px;height: 300px;"
                        alt="profilePic">

                </div>

                <div class="profile-user-settings">

                    <h1 class="profile-user-name">{{ userData.username }}</h1>

                    <div v-if="userData.isUserPage">
                        <div class="containter">
                            <ckeditor :editor="editor" v-model="userDescription">
                            </ckeditor>
                        </div>
                        <button @click="addDescription" class="button-9" style="width:20%" role="button">
                            {{ !userData.description ? 'Add description' : 'Edit description' }}
                        </button>
                    </div>
                    <div v-else>
                        Description:
                        <div v-html="userData.description">

                        </div>
                    </div>


                </div>

                <div class="profile-stats">

                    <ul>
                        <li><span class="profile-stat-count">{{ userPosts.length }}</span> posts</li>
                    </ul>

                </div>


            </div>
            <!-- End of profile section -->

        </div>
        <!-- End of container -->

        <div class="container">
            <div v-if="!userPosts" class="loader"></div>
            <div class="gallery">
                <div v-if="userData.isUserPage" @click="showModal" class="gallery-item" tabindex="0">

                    <img src="@/assets/default-image.jpg" class="gallery-image" alt="">

                    <div class="gallery-item-info">
                        <ul>
                            <li class="gallery-item-likes"><span class="visually-hidden">Add photo:</span>
                                <fa icon="plus"></fa>Add photo
                            </li>
                        </ul>
                    </div>

                </div>
                <div class="modal fade" id="newPhotoModal" tabindex="-1">
                    <div class="modal-dialog">
                        <div v-if="!postModal" class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">New post</h5>
                                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                            </div>
                            <input accept="image/jpeg, image/png ,image/gif" v-on:change="changeModalImg" id="imageInput"
                                class="d-none" type="file">
                            <div class="modal-body">
                                <form class="container">
                                    <div @click="openDialog" class="gallery-item" tabindex="0">

                                        <img src="@/assets/default-image.jpg" style="max-width: 400px;max-height: 500px;"
                                            id="modalImg" class="gallery-image" alt="">

                                        <div class="gallery-item-info">
                                            <ul>
                                                <li class="gallery-item-likes"><span class="visually-hidden"></span>
                                                    Click to add photo
                                                </li>
                                            </ul>
                                        </div>

                                    </div>
                                    <ckeditor :editor="editor" v-model="editorData">
                                    </ckeditor>

                                    <div class="error"> {{ this.errorMessage }}</div>
                                    <button class="button-9" @click.prevent="postNewPost" role="button">Post</button>
                                </form>
                            </div>
                        </div>
                        <Post v-else class="modal-content" :userProfilePic="userData.userProfilePicture"
                            :postData="selectedPostData" :isModal="true" :updateMethod="getUserPosts" />
                    </div>
                </div>

                <div v-for="item in userPosts">
                    <div @click="openPost(item)" class="gallery-item" tabindex="0">

                        <img :src="'https://localhost:7006' + item.postUrl" class="gallery-image" alt="">

                        <div class="gallery-item-info">

                            <ul>

                                <li class="gallery-item-likes"><span class="visually-hidden">Likes:</span>
                                    <fa icon="heart"></fa> {{ item.postLikes }}
                                </li>
                                <li class="gallery-item-comments"><span class="visually-hidden">Comments:</span>
                                    <fa icon="comment"></fa> {{ item.postComments }}
                                </li>
                            </ul>

                        </div>

                    </div>
                </div>
            </div>
            <!-- End of gallery -->



        </div>
    </div>

    <!-- End of container -->
</template>
<script>
import Post from '@/components/Post.vue'
import Navbar from '@/components/Navbar.vue'
import { Modal } from 'bootstrap'
import ClassicEditor from '@ckeditor/ckeditor5-build-classic'
import axios from 'axios'

export default {
    components: {
        Navbar,
        Post
    },
    data() {
        return {
            userPosts: {},
            userData: {},
            errorMessage: '',
            myModal: null,
            userDescription: '',
            selectedPostData: null,
            postModal: false,
            newPostPic: null,
            profilePic: '',
            defaultImgPath: '',
            editor: ClassicEditor,
            editorData: '',
            data: {
                "url": '',
                "description": '',
                "likes": '',
                "comments": '',
            }
        }
    },
    mounted() {
        this.profilePic = localStorage.getItem('profilePic')
        this.myModal = new Modal(document.getElementById('newPhotoModal'))
        this.getUserPosts()
    },
    watch: {
        '$route'(to, from) {
            this.profilePic = localStorage.getItem('profilePic')
            this.myModal = new Modal(document.getElementById('newPhotoModal'))
            this.getUserPosts()
        }
    },
    methods: {
        addDescription() {
            const formData = new FormData();
            formData.append('description', this.userDescription)
            axios({
                method: 'PUT',
                data: formData,
                url: 'https://localhost:7006/api/Users/update-description',
                headers: {
                    'Accept': " */*",
                    'Content-Type': `multipart/form-data`,
                    'Authorization': 'Bearer ' + localStorage.getItem('token')
                },
            }).catch(error => {
                this.onSubmitError = 'Something went wrong!'
                console.log(error)
            });
        },
        showModal() {
            this.errorMessage = ''
            this.postModal = false
            this.myModal.show()
        },
        openDialog() {
            document.getElementById('imageInput').click()
        },
        openPost(postData) {
            this.selectedPostData = postData
            this.postModal = true
            this.myModal.show()
        },
        changeModalImg(event) {
            this.newPostPic = event.target.files[0];
            const file = event.target.files[0];
            const reader = new FileReader();
            if (file) {
                reader.readAsDataURL(file);
                reader.onload = () => {
                    const dataUrl = reader.result;
                    this.defaultImgPath = document.querySelector("#modalImg").src
                    document.querySelector("#modalImg").src = dataUrl;

                }
            }

        },
        getUserPosts() {
            axios("https://localhost:7006/api/Posts/userPosts/" + this.$route.params.id, {
                method: 'get',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + localStorage.getItem('token')
                }
            }).then(res => {
                console.log(res.data)
                this.userPosts = res.data.query
                this.userData = res.data.userInfoDict
                if (this.userData.isUserPage && this.userData.description) {
                    this.userDescription = this.userData.description
                }
            })
        },
        postNewPost() {
            if (this.editorData === '' || this.newPostPic === null) {
                this.errorMessage = "You need to fill both description and image inputs!"
            }
            else {
                const formData = new FormData();
                formData.append('Id', 0);
                formData.append('CreatorId', 0);
                formData.append('Likes', 0);
                formData.append('Url', 'url');
                formData.append('Description', this.editorData);
                formData.append('Comments', 0);
                formData.append('Image', this.newPostPic);

                axios({
                    method: 'POST',
                    url: 'https://localhost:7006/api/Posts/add-post',
                    data: formData,
                    headers: {
                        'Accept': " */*",
                        'Content-Type': `multipart/form-data`,
                        'Authorization': 'Bearer ' + localStorage.getItem('token')
                    },
                }).then(() => {
                    this.editorData = ''
                    this.newPostPic = null
                    document.querySelector("#modalImg").src = this.defaultImgPath
                    document.querySelector('.btn-close').click()
                    this.getUserPosts()

                }).catch(error => {
                    this.onSubmitError = 'Something went wrong!'
                    console.log(error)
                });
            }

        }
    }


}
</script>

<style scoped>
.button-9 {
    appearance: button;
    backface-visibility: hidden;
    background: linear-gradient(90deg, rgba(210, 210, 75, 1) 0%, rgba(221, 149, 20, 1) 35%, rgba(139, 0, 255, 1) 100%);
    border-radius: 6px;
    border-width: 0;
    box-shadow: rgba(50, 50, 93, .1) 0 0 0 1px inset, rgba(50, 50, 93, .1) 0 2px 5px 0, rgba(0, 0, 0, .07) 0 1px 1px 0;
    box-sizing: border-box;
    color: #fff;
    cursor: pointer;
    font-family: -apple-system, system-ui, "Segoe UI", Roboto, "Helvetica Neue", Ubuntu, sans-serif;
    font-size: 100%;
    height: 44px;
    line-height: 1.15;
    margin: 12px 0 0;
    outline: none;
    overflow: hidden;
    padding: 0 25px;
    position: relative;
    text-align: center;
    text-transform: none;
    transform: translateZ(0);
    transition: all .2s, box-shadow .08s ease-in;
    user-select: none;
    -webkit-user-select: none;
    touch-action: manipulation;
    width: 100%;
}

.button-9:disabled {
    cursor: default;
}

.button-9:focus {
    box-shadow: rgba(50, 50, 93, .1) 0 0 0 1px inset, rgba(50, 50, 93, .2) 0 6px 15px 0, rgba(0, 0, 0, .1) 0 2px 2px 0, rgba(50, 151, 211, .3) 0 0 0 4px;
}

img {
    display: block;
}

.container {
    max-width: 87.5rem;
    margin: 0 auto;
    padding: 0 2rem;
}

.pageContainer {
    max-width: 80.5rem;
    margin: 0 auto;
    padding: 0 2rem;
}

.btn {
    display: inline-block;
    font: inherit;
    background: none;
    border: none;
    color: inherit;
    padding: 0;
    cursor: pointer;
}

.btn:focus {
    outline: 0.5rem auto #4d90fe;
}

.visually-hidden {
    position: absolute !important;
    height: 1px;
    width: 1px;
    overflow: hidden;
    clip: rect(1px, 1px, 1px, 1px);
}

/* Profile Section */

.profile {
    padding: 5rem 0;
}

.profile::after {
    content: "";
    display: block;
    clear: both;
}

.profile-image {
    float: left;
    width: calc(33.333% - 1rem);
    display: flex;
    justify-content: center;
    align-items: center;
    margin-right: 3rem;
}

.profile-image img {
    border-radius: 50%;
}

.profile-user-settings,
.profile-stats,
.profile-bio {
    float: left;
    width: calc(66.666% - 2rem);
}

.profile-user-settings {
    margin-top: 1.1rem;
}

.profile-user-name {
    display: inline-block;
    font-size: 3.2rem;
    font-weight: 300;
}

.profile-edit-btn {
    font-size: 1.4rem;
    line-height: 1.8;
    border: 0.1rem solid #dbdbdb;
    border-radius: 0.3rem;
    padding: 0 2.4rem;
    margin-left: 2rem;
}

.profile-settings-btn {
    font-size: 2rem;
    margin-left: 1rem;
}

.profile-stats {
    margin-top: 2.3rem;
}

.profile-stats li {
    display: inline-block;
    font-size: 1.6rem;
    line-height: 1.5;
    margin-right: 4rem;
    cursor: pointer;
}

.profile-stats li:last-of-type {
    margin-right: 0;
}

.profile-bio {
    font-size: 1.6rem;
    font-weight: 400;
    line-height: 1.5;
    margin-top: 2.3rem;
}

.profile-real-name,
.profile-stat-count,
.profile-edit-btn {
    font-weight: 600;
}

/* Gallery Section */

.gallery {
    display: flex;
    flex-wrap: wrap;
    margin: -1rem -1rem;
    padding-bottom: 3rem;
}

.gallery-item {
    position: relative;
    flex: 1 0 22rem;
    margin: 1rem;
    color: #fff;
    cursor: pointer;
    max-width: 400px;
}

.gallery-item:hover .gallery-item-info,
.gallery-item:focus .gallery-item-info {
    display: flex;
    justify-content: center;
    align-items: center;
    position: absolute;
    top: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.3);
}

.gallery-item-info {
    display: none;
}

.gallery-item-info li {
    display: inline-block;
    font-size: 1.7rem;
    font-weight: 600;
}

.gallery-item-likes {
    margin-right: 2.2rem;
}

.gallery-item-type {
    position: absolute;
    top: 1rem;
    right: 1rem;
    font-size: 2.5rem;
    text-shadow: 0.2rem 0.2rem 0.2rem rgba(0, 0, 0, 0.1);
}

.fa-clone,
.fa-comment {
    transform: rotateY(180deg);
}

.gallery-image {
    width: 100%;
    height: 100%;
    object-fit: cover;
}

/* Loader */

.loader {
    width: 5rem;
    height: 5rem;
    border: 0.6rem solid #999;
    border-bottom-color: transparent;
    border-radius: 50%;
    margin: 0 auto;
    animation: loader 500ms linear infinite;
}

/* Media Query */

@media screen and (max-width: 40rem) {
    .profile {
        display: flex;
        flex-wrap: wrap;
        padding: 4rem 0;
    }

    .profile::after {
        display: none;
    }

    .profile-image,
    .profile-user-settings,
    .profile-bio,
    .profile-stats {
        float: none;
        width: auto;
    }

    .profile-image img {
        width: 7.7rem;
    }

    .profile-user-settings {
        flex-basis: calc(100% - 10.7rem);
        display: flex;
        flex-wrap: wrap;
        margin-top: 1rem;
    }

    .profile-user-name {
        font-size: 2.2rem;
    }

    .profile-edit-btn {
        order: 1;
        padding: 0;
        text-align: center;
        margin-top: 1rem;
    }

    .profile-edit-btn {
        margin-left: 0;
    }

    .profile-bio {
        font-size: 1.4rem;
        margin-top: 1.5rem;
    }

    .profile-edit-btn,
    .profile-bio,
    .profile-stats {
        flex-basis: 100%;
    }

    .profile-stats {
        order: 1;
        margin-top: 1.5rem;
    }

    .profile-stats ul {
        display: flex;
        text-align: center;
        padding: 1.2rem 0;
        border-top: 0.1rem solid #dadada;
        border-bottom: 0.1rem solid #dadada;
    }

    .profile-stats li {
        font-size: 1.4rem;
        flex: 1;
        margin: 0;
    }

    .profile-stat-count {
        display: block;
    }
}

/* Spinner Animation */

@keyframes loader {
    to {
        transform: rotate(360deg);
    }
}

/*

The following code will only run if your browser supports CSS grid.

Remove or comment-out the code block below to see how the browser will fall-back to flexbox & floated styling. 

*/

@supports (display: grid) {
    .profile {
        display: grid;
        grid-template-columns: 1fr 2fr;
        grid-template-rows: repeat(3, auto);
        grid-column-gap: 3rem;
        align-items: center;
    }

    .profile-image {
        grid-row: 1 / -1;
    }

    .gallery {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(22rem, 1fr));
        grid-gap: 2rem;
    }

    .profile-image,
    .profile-user-settings,
    .profile-stats,
    .profile-bio,
    .gallery-item,
    .gallery {
        width: auto;
        margin: 0;
    }

    @media (max-width: 40rem) {
        .profile {
            grid-template-columns: auto 1fr;
            grid-row-gap: 1.5rem;
        }

        .profile-image {
            grid-row: 1 / 2;
        }

        .profile-user-settings {
            display: grid;
            grid-template-columns: auto 1fr;
            grid-gap: 1rem;
        }

        .profile-edit-btn,
        .profile-stats,
        .profile-bio {
            grid-column: 1 / -1;
        }

        .profile-user-settings,
        .profile-edit-btn,
        .profile-settings-btn,
        .profile-bio,
        .profile-stats {
            margin: 0;
        }
    }
}
</style>