<template>
    <div v-if="showPost" class="card" style="max-width: 600px;margin: 10%;">
        <div class="card-header">
            <button type="button" class="btn-close 	d-none" data-bs-dismiss="modal" aria-label="Close"></button>
            <div class="d-flex align-items-center justify-content-between">
                <div class="d-flex align-items-center" @click="viewProfile(postData.postCreatorId)">
                    <img :src="'https://localhost:7006' + this.userProfilePic" class="profileImage profilePic">
                    <h6 class="mb-0 ms-2" data-bs-toggle="popover" data-bs-trigger="hover focus"
                        title="<div class='d-flex'><img src='justinProfile.png' class='profileImage'> <div class='ms-2'><div>Justin Bieber</div><div>Sub-text</div></div>"
                        data-bs-content="And here's some amazing content. It's very engaging. Right?">
                        {{ this.postData.userUsername }}</h6>

                </div>
                <div v-if="postData.isCreatedByCurrentUser" class="dropdown">
                    <img @click="toggleDropdown" src="@/assets/more.png" style="width:20px;height:20px">
                    <ul class="dropdown-menu" :class="{ 'show': isOpen }">
                        <li>
                            <a @click="toggleEdit">Edit</a>
                            <a @click="deleteComponent">Delete</a>
                        </li>

                    </ul>
                </div>

            </div>
        </div>
        <div v-if="showEditOptions" class="container">
            <img class="card-img-top" :src="'https://localhost:7006' + this.postData.postUrl" id="editPhoto"
                alt="Card image cap">
            <div class="text-overlay">
                <h2 @click="selectNewImage">Click on the text to edit image<input accept="image/jpeg, image/png ,image/gif"
                        v-on:change="editPostPhoto" id="imageInput" class="d-none" type="file"></h2>
            </div>
        </div>
        <div v-else>
            <img class="card-img-top" :src="'https://localhost:7006' + this.postData.postUrl"
                :id="'img' + this.postData.postId" alt="Card image cap">
        </div>
        <div class="card-footer" style="position: relative;">
            <div v-if="!showEditOptions" class="d-flex align-items-center justify-content-between mb-3">
                <div class="stage">
                    <div @click="likePost" class="heart" :class="{ 'is-active': this.postData.isLiked }">
                    </div>
                </div>
            </div>
            <div class="text-container">
                <div>
                    <div v-if="showEditOptions">
                        <ckeditor :editor="editor" v-model="editEditorData">
                        </ckeditor>
                        <button style="margin-top: 5px;" class="btn" @click="editPost">Confirm
                            edit</button>
                    </div>
                    <div v-else>
                        <div class="d-flex align-items-center">
                            <span style="font-weight: bolder;">Likes {{ this.postData.postLikes }}</span>
                        </div>
                        <div v-html="postData.postDescription" id="description">

                        </div>

                        <h5 @click="toggleComments" style="font: bolder;">
                            View all {{ this.postData.postComments }} comments
                        </h5>
                        <div v-if="showComments" style="padding-top:10px ;">
                            <div v-for="comment in comments">
                                <span class="comment-element" style="padding-bottom: 30px;"><img
                                        :src="'https://localhost:7006' + comment.userProfilePicture"
                                        class="profileImage profilePic"></span>
                                <span class="comment-element">
                                    <div style="font-weight: bolder;">{{ comment.userUsername }}</div>
                                    {{ comment.commentContent }}
                                    <span>
                                        <button class="btn" @click="likeComment(comment)">
                                            Like
                                            <fa :icon="comment.isLiked ? ['fas', 'heart'] : ['far', 'heart']"
                                                class="fa-heart-border"></fa>
                                            {{ comment.commentLikes }}
                                        </button>

                                    </span>

                                    <span v-if="comment.isCreatedByUser">
                                        <button @click="removeComment(comment)" class="btn"> Delete
                                            <fa icon="trash"></fa>
                                        </button>
                                    </span>
                                    <div class="text-secondary">
                                        Posted on {{ comment.dateCreated.replace(/T00:00:00$/, '') }}
                                    </div>
                                </span>


                            </div>
                        </div>
                        <div class="error">{{ this.errorMessage }}</div>
                        <div class="d-flex align-items-center mt-3">

                            <div class="flex-fill mx-2">
                                <input v-model="commentText" type="text" class="form-control">
                            </div>

                            <div>
                                <button @click="addComment" class="btn">Post comment</button>
                            </div>
                        </div>
                        <div>
                            <small class="text-secondary">Posted on {{ this.postData.postCreationDate.replace(/T00:00:00$/,
                                '')
                            }}</small>
                        </div>
                    </div>


                </div>

            </div>

        </div>
    </div>
</template>
<script>
import axios from 'axios'
import ClassicEditor from '@ckeditor/ckeditor5-build-classic'
export default {
    props: ['postData', 'userProfilePic', 'isModal', 'updateMethod'],
    data() {
        return {
            showPost: true,
            errorMessage: '',
            showEditOptions: false,
            editor: ClassicEditor,
            editPostPic: null,
            editEditorData: this.postData.postDescription,
            isOpen: false,
            showComments: false,
            commentText: '',
            comments: []
        }
    }, mounted() {
        this.getComments()
    },
    methods: {
        toggleDropdown() {
            this.isOpen = !this.isOpen;
        },
        toggleComments() {
            this.showComments = !this.showComments
        }, toggleEdit() {
            this.toggleDropdown()
            this.showEditOptions = !this.showEditOptions
        }, selectNewImage() {
            document.getElementById('imageInput').click()
        }, editPostPhoto(event) {
            this.editPostPic = event.target.files[0];
            const file = event.target.files[0];
            const reader = new FileReader();
            if (file) {
                reader.readAsDataURL(file);
                reader.onload = () => {
                    document.querySelector("#editPhoto").src = reader.result;

                }
            }
        }, editPost() {
            const formData = new FormData();
            formData.append('Id', this.postData.postId);
            formData.append('CreatorId', 0);
            formData.append('Likes', 0);
            formData.append('Url', 'url');
            formData.append('Description', this.editEditorData);
            formData.append('Comments', 0);
            formData.append('Image', this.editPostPic);

            axios({
                method: 'PUT',
                data: formData,
                url: 'https://localhost:7006/api/Posts/' + this.postData.postId,
                headers: {
                    'Accept': " */*",
                    'Content-Type': `multipart/form-data`,
                    'Authorization': 'Bearer ' + localStorage.getItem('token')
                },
            }).then(() => {
                const file = this.editPostPic
                const reader = new FileReader();
                if (file) {
                    reader.readAsDataURL(file);
                    reader.onload = () => {
                        document.querySelector("#img" + this.postData.postId).src = reader.result;

                    }
                }
                this.postData.postDescription = this.editEditorData
                this.showEditOptions = !this.showEditOptions

            }).catch(error => {
                this.onSubmitError = 'Something went wrong!'
                console.log(error)
            });
        },
        deleteComponent() {
            axios({
                method: 'DELETE',
                url: 'https://localhost:7006/api/Posts/' + this.postData.postId,
                headers: {
                    'Accept': " */*",
                    'Content-Type': `multipart/form-data`,
                    'Authorization': 'Bearer ' + localStorage.getItem('token')
                },
            }).then(() => {
                this.updateMethod()
                if (this.isModal) {
                    document.querySelector('.btn-close').click()
                }
                else {
                    this.showPost = false
                }
            }).catch(error => {
                this.onSubmitError = 'Something went wrong!'
                console.log(error)
            });

        },
        viewProfile(id) {
            this.$router.push({ name: 'account', params: { id: id } });
        },
        likePost(event) {
            const clickedElement = event.target;
            clickedElement.classList.toggle('is-active');

            axios({
                method: 'POST',
                url: 'https://localhost:7006/api/Posts/toggle-like-post/' + this.postData.postId,
                headers: {
                    'Accept': " */*",
                    'Content-Type': `multipart/form-data`,
                    'Authorization': 'Bearer ' + localStorage.getItem('token')
                },
            }).then(() => {
                this.postData.postLikes += this.postData.isLiked ? -1 : 1;
                this.postData.isLiked = !this.postData.isLiked;
            }).catch(error => {
                this.onSubmitError = 'Something went wrong!'
                console.log(error)
            });






        },
        likeComment(comment) {
            comment.isLiked = !comment.isLiked
            comment.commentLikes += comment.isLiked ? 1 : -1;

            axios({
                method: 'POST',
                url: 'https://localhost:7006/api/Comments/toggle-like-comment/' + comment.commentId,
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
        getComments() {
            axios("https://localhost:7006/api/Comments/" + this.postData.postId, {
                method: 'GET',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + localStorage.getItem('token')
                }
            }).then(res => {
                this.comments = res.data.query
            })
        },
        addComment() {
            if (this.commentText !== '') {
                this.errorMessage = ''
                const formData = new FormData()
                formData.append('content', this.commentText)
                axios({
                    method: 'POST',
                    data: formData,
                    url: 'https://localhost:7006/api/Comments/add-comment/' + this.postData.postId,
                    headers: {
                        'Accept': " */*",
                        'Content-Type': `multipart/form-data`,
                        'Authorization': 'Bearer ' + localStorage.getItem('token')
                    },
                }).then(() => {
                    this.commentText = ''
                    this.postData.postComments++
                    this.getComments()
                }).catch(error => {
                    this.onSubmitError = 'Something went wrong!'
                    console.log(error)
                });
            }
            else {
                this.errorMessage = 'Empthy comment no allowed!'
            }


        },
        removeComment(comment) {
            axios({
                method: 'DELETE',
                url: 'https://localhost:7006/api/Comments/' + comment.commentId,
                headers: {
                    'Accept': " */*",
                    'Content-Type': `multipart/form-data`,
                    'Authorization': 'Bearer ' + localStorage.getItem('token')
                },
            }).then(() => {
                this.postData.postComments--
                this.getComments()
            }).catch(error => {
                this.onSubmitError = 'Something went wrong!'
                console.log(error)
            });
        }
    }

}
</script>
<style lang="scss" scoped>
.container {
    position: relative;
    display: inline-block;
}

.text-overlay {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    color: #fff;
    text-align: center;
    background-color: rgba(0, 0, 0, 0.7);
}

.comment-element {
    display: inline-block;
    margin-right: 10px;
}

.dropdown {
    position: relative;
    display: inline-block;
}

.dropdown .dropdown-menu {
    display: none;
    position: absolute;
    z-index: 1;
    background-color: #f9f9f9;
    min-width: 80px;
    padding: 8px;
    border: 1px solid #ccc;
    border-radius: 4px;
    transition: max-height 0.2s ease-out;
}

.dropdown .dropdown-menu.show {
    display: block;
    max-height: 200px;
    width: 10px;
    overflow-y: auto;
}

.dropdown .dropdown-menu a {
    display: block;
    padding: 4px 0;
    text-decoration: none;
    color: #333;
}

.btn {
    background-color: #f9f9f9;
    border: 1px solid #ccc;
    border-radius: 4px;
    padding: 4px 8px;
    cursor: pointer;
}

.profilePic {
    width: 30px;
    height: 30px;
    display: block;
    border-radius: 50%;
    object-fit: cover;
    border: 3px solid #be4c2f;
}



.fa-heart {
    color: red;
    cursor: pointer;
}



.heart {
    width: 100px;
    height: 100px;
    background: url("@/assets/heart.png") no-repeat;
    background-position: 0 0;
    cursor: pointer;
    transition: background-position 1s steps(28);
    transition-duration: 0s;
    position: absolute;
    top: -25px;
    left: -25px;
    z-index: 2;

    &.is-active {
        transition-duration: 1s;
        background-position: -2800px 0;
    }
}

.text-container {
    position: relative;
    z-index: 1;
    padding-top: 30px;
    /* Add any other necessary styles */
}

.stage {
    width: 20px;
}
</style>