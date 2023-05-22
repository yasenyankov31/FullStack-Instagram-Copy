<template>
    <div class="navigation" style="z-index: 3">
        <div class="logo">
            <RouterLink :to="{ name: 'home' }" style="padding-right:20px">
                Instagram
            </RouterLink>
        </div>
        <div class="navigation-search-container">
            <i class="fa fa-search"></i>

            <div class="search-container">
                <div class="search-container-box">
                    <div class="dropdown">
                        <input class="search-field" v-model="searchText" type="text" placeholder="Search">
                        <div class="dropdown-content">
                            <div v-for="user in filteredItems">
                                <RouterLink @click="resetSearchText" :to="{ name: 'account', params: { id: user.userId } }"
                                    class="navigation-link" style="padding-right:20px">
                                    <img :src="'https://localhost:7006' + user.userProfilePic" class="profilePic"
                                        alt="Profile Image">
                                    {{ user.userUsername }}
                                </RouterLink>

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="navigation-icons">
            <RouterLink v-if="interface == 0" :to="{ name: 'users' }" style="padding-right:20px">
                <img height="30" src="@/assets/users.png" alt="users">
            </RouterLink>
            <RouterLink :to="{ name: 'home' }" style="padding-right:20px">
                <img height="30" src="@/assets/home.png" alt="home">
            </RouterLink>
            <RouterLink :to="{ name: 'account', params: { id: 0 } }" class="navigation-link" style="padding-right:20px">
                <img :src="'https://localhost:7006' + profilePic" class="profilePic" alt="Profile Image">
            </RouterLink>
            <RouterLink @click="logOut" :to="{ name: 'login' }" class="navigation-link" style="padding-right:20px">
                <img src="@/assets/logout.png" style="width: 30px;height: 30px;" alt="Log out">
            </RouterLink>
        </div>
    </div>
</template>
<script>
import axios from 'axios'
export default {
    data() {
        return {
            profilePic: '',
            searchText: '',
            interface: 1,
            allUsers: []
        }
    },
    computed: {
        filteredItems() {
            if (this.searchText === '') {
                return []
            } else {
                const searchLower = this.searchText.toLowerCase();
                return this.allUsers.filter(item =>
                    item.userUsername.toLowerCase().includes(searchLower)
                );
            }
        }
    },
    mounted() {
        this.interface = localStorage.getItem('interface')
        this.profilePic = localStorage.getItem('profilePic')
        axios("https://localhost:7006/api/Users", {
            method: 'GET',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem('token')
            }
        }).then(res => {
            this.allUsers = res.data.query
        })
    },
    methods: {
        resetSearchText() {
            this.searchText = ''
        },
        logOut() {
            localStorage.setItem('token', '')
        }
    }
}
</script>

<style scoped>
.dropdown {
    position: relative;
    display: inline-block;
}

.dropdown-content {
    display: block;
    position: absolute;
    background-color: #f9f9f9;
    min-width: 160px;
    box-shadow: 0px 8px 16px 0px rgba(0, 0, 0, 0.2);
}

.dropdown-content a {
    color: black;
    padding: 12px 16px;
    text-decoration: none;
    display: block;
}

.dropdown:hover .dropdown-content {
    display: block;
}

.btn-color {
    background: linear-gradient(90deg, rgba(210, 210, 75, 1) 0%, rgba(221, 149, 20, 1) 35%, rgba(139, 0, 255, 1) 100%) !important;
}

.navigation {
    background-color: #ffffff;
    height: 80px;
    position: fixed;
    width: 100%;
    top: 0;
    left: 0;
    border-bottom: 1px solid rgba(0, 0, 0, 0.0975);
    display: flex;
    align-items: center;
    justify-content: space-around;
    padding: 0px 50px;
    box-sizing: border-box;
    z-index: 2;
    /* animation magic */
    transition: all 0.4s ease-in-out;
    -webkit-transition: all 0.4s ease-in-out;
    -moz-transition: all 0.4s ease-in-out;
}

.shrink {
    height: 50px;
}

.navigation .logo a {
    position: relative;
    color: #000000;
    font-size: 30px;
    font-family: "billabong", sans-serif;
    text-decoration: none;
}

.navigation-search-container {
    position: relative;
}

.navigation-search-container input {
    background-color: #fafafa;
    padding: 3px 20px;
    padding-left: 25px;
    height: 30px;
    box-sizing: border-box;
    border: 1px solid rgba(0, 0, 0, 0.0975);
    border-radius: 3px;
    font-size: 14px;
}

.navigation-search-container .fa-search {
    position: absolute;
    top: 10px;
    left: 10px;
    font-size: 11px;
    color: rgba(0, 0, 0, 0.5);
}

@media only screen and (min-width: 320px) and (max-width: 650px) {

    /* Navigation */
    .navigation {
        padding: 0 20px;
        justify-content: space-between;
    }

    .navigation-search-container {
        display: none;
    }

    .navigation-icons {
        display: flex;
    }
}

.navigation-icons {
    display: flex;
}

.navigation-search-container input:focus {
    outline: none;
}

.navigation-search-container input::placeholder {
    text-align: center;
}

.navigation-icons a {
    text-decoration: none;
}

.navigation-link i {
    margin-left: 30px;
    color: black;
    text-decoration: none;
    font-size: 22px;
}

.notification-bubble-wrapper {
    position: relative;
    top: -30px;
    left: 17px;
}

.notification-bubble {
    position: absolute;
    min-width: 20px;
    min-height: 20px;
    border-radius: 50%;
    background: #ff2c74;
    color: #fff;
    text-align: center;
    font-size: 13px;
    padding: 5px 5px 3px;
    font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Oxygen,
        Ubuntu, Cantarell, "Open Sans", "Helvetica Neue", sans-serif;
    font-weight: 500;
    display: none;
}

.notification-bubble {
    user-select: none;
    -moz-user-select: none;
    -webkit-user-select: none;
    -ms-user-select: none;
}

.profilePic {
    width: 40px;
    height: 40px;
    display: block;
    border-radius: 50%;
    object-fit: cover;
    border: 3px solid #be4c2f;
}

.padIcons {
    padding-top: 10px;
    padding-right: 25px;
}

.searchBar {
    display: block;
    padding: 0.375rem 0.75rem;
    font-size: 1rem;
    font-weight: 400;
    line-height: 1.5;
    color: #212529;
    background-color: #fff;
    background-clip: padding-box;
    border: 1px solid #ced4da;
    -webkit-appearance: none;
    -moz-appearance: none;
    appearance: none;
    border-radius: 0.375rem;
    transition: border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
}
</style>