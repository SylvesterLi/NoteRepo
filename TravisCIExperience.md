## Travis CI是什么？

![](/PicSource/TravisPics/intr.png)

## 准备一下：

- 一个Hexo源码仓库（放在名为"HexoCode"的分支）,就像这样：

![](/PicSource/TravisPics/branch_choose.png)

- 获取到你的GitHub_Token
> 点这里： [https://github.com/settings/tokens](https://github.com/settings/tokens)

权限保持默认就行，token一定要保存好，等会要在Travis CI里用，它只会显示一次。
![](/PicSource/TravisPics/token-new.png)

![](/PicSource/TravisPics/gh-token.png)

- 进入Travis CI,打开权限

> 点这里： [https://travis-ci.org/](https://travis-ci.org/)

进入设置(Settings)，打开你要自动编译的公开仓库。

![](/PicSource/TravisPics/sync.png)

然后点击按钮旁边的Settings，找到环境变量(Environment Variables)

左侧随便填个名字。如果你怕出错啥的就跟我一样填Travis，右侧的密码一样的东西填刚刚我们获取到的GitHub_Token

我的是：1a5xxxx0cb0b3xxxxxx631f03c7xxxx73020fb6d

这个很重要的！

![](/PicSource/TravisPics/sync1.png)

## 好，接下来我们就要给Travis留言，告诉他要怎么样编译这些东西。

- 我们要创建一个名为``` .travis.yml ```的文件，用记事本打开就可以。

```yml
language: node_js
node_js: stable

cache:
  directories:
    - node_modules

branches:
  only:
    - hexo

before_install:
  - npm install -g hexo-cli
  

# S: Build Lifecycle
install:
  - npm install
  - npm install hexo-deployer-git --save


# before_script:
 # - npm install -g gulp
    
script:
  - hexo clean
  - hexo generate

after_script:
#  - cd ./public
#  - git init
  - git config user.name "gattia"
  - git config user.email "gattia.su@gmail.com"
#  - git add .
#  - git commit -m "Update docs"
#  - git push --force --quiet "${Travis}@${GH_REF}" master:master
  - sed -i "s/Travis/${Travis}/g" ./_config.yml
  - hexo deploy

```

![](/PicSource/TravisPics/script.jpg)

有没有感觉Travis CI就像是一台远程计算机？

将``` .travis.yml ``` 与 ``` _config.yml ``` 放在一起。

![](/PicSource/TravisPics/sm_dic.png)

还要修改下_config.yml文件的deploy节点：

```yml
# 修改前
deploy:
  - type: git
    repo: git@github.com:SylvesterLi/HiTravi.git
    branch: master
```

```yml
# 修改后
deploy:
- type: git
  # 下方的gh_token会被.travis.yml中sed命令替换
  repo: https://Travis@github.com/SylvesterLi/HiTravi.git
  branch: master
```

如果有其他问题，可以参考一下我的GitHub中HiTravi仓库，这个仓库Travis CI运行正常.

## 接下来我说一下我遇到的几个问题。

### 壹  

一开始我也不知道怎么配置Travis CI，参考了好多文章，各有各的写法，但是我总是不成功。后来按照其中一个完完整整的做完就成功了。

> 可以参考这篇：[https://blog.csdn.net/Xiong_IT/article/details/78675874](https://blog.csdn.net/Xiong_IT/article/details/78675874)

这篇讲的还是比较清楚的，就是sed那个地方我一开始没看懂。

### 贰

出于个人习惯，我不愿意把编译出来的文件放在master分支。我想把博客源代码放在master，编译文件放在gh-pages分支。所以在

```yml
branches:
  only:
    - hexo
```

跟[另一篇博客中](https://www.jianshu.com/p/5691815b81b6)的

```yml
#  - git add .
#  - git commit -m "Update docs"
#  - git push --force --quiet "${Travis}@${GH_REF}" master:master
```

混乱了半天。

后来做完才明白，想监视哪个分支就设置only:这里的，想推送到哪，就去设置_config.yml中的deploy:branch。

### 叁

说到这里得提一下昨天查错的过程。

第一次提交上去的时候我以为成功了：

![](/PicSource/TravisPics/mis1.png)

结果去GitHub一查，仓库没变化，翻到下面看到：

![](/PicSource/TravisPics/mis2.png)

以为是网址错了还是分支名错了，还是没权限访问：

![](/PicSource/TravisPics/mis3.png)

直到我发现，仓库名错了哈哈哈哈，然后改了还是访问不到：

![](/PicSource/TravisPics/mis4.png)

索性就改了sed命令那个教程：

![](/PicSource/TravisPics/mis5.png)

之前我的Repo名字叫HiTravis，当sed命令去替换_config.yml文件里的Travis时顺手就把我仓库的名字给替换掉了。所以我赶紧改了个仓库名。

![](/PicSource/TravisPics/done.png)

大功告成！！

![](/PicSource/TravisPics/res.png)

## 前前后后，构建了十三次，Travis不容易，熬夜不值得。



