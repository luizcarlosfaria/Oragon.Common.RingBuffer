pipeline {
    
    agent none

    environment {
        COMPOSE_PROJECT_NAME = "${env.JOB_NAME}-${env.BUILD_ID}"
    }
    stages {
      
        stage('Build') {

            agent {
                dockerfile {
                    // alwaysPull false
                    // image 'microsoft/dotnet:2.1-sdk'
                    // reuseNode false
                    args '-u root:root'
                }
            }

            steps {
                
                echo sh(script: 'env|sort', returnStdout: true)

                sh 'dotnet build ./Oragon.Common.RingBuffer.sln'

            }

        }
        
        stage('Setup databases') {
            
            agent any
            
            steps {

                    sh  '''

                        #docker-compose up -d

                        #sleep 60

                        '''
            }

        }


        stage('Test') {

            agent {
                dockerfile {
                    // alwaysPull false
                    // image 'microsoft/dotnet:2.1-sdk'
                    // reuseNode false
                    args '-u root:root -v /var/run/docker.sock:/var/run/docker.sock'
                }
            }

            steps {

                 withCredentials([usernamePassword(credentialsId: 'SonarQube', passwordVariable: 'SONARQUBE_KEY', usernameVariable: 'DUMMY' )]) {

                    sh  '''

                        #docker-compose up -d

                        #sleep 190

                        export PATH="$PATH:/root/.dotnet/tools"

                        dotnet test ./src/Oragon.Common.RingBuffer.Tests/Oragon.Common.RingBuffer.Tests.csproj \
                            --configuration Debug \
                            --output ../output-tests  \
                            /p:CollectCoverage=true \
                            /p:CoverletOutputFormat=opencover \
                            /p:CoverletOutput='/output-coverage/coverage.xml' \
                            /p:Exclude="[Oragon.*.Tests]*"

                        dotnet sonarscanner begin /k:"Oragon-RingBuffer" \
                            /d:sonar.host.url="http://sonar.oragon.io" \
                            /d:sonar.login="$SONARQUBE_KEY" \
                            /d:sonar.cs.opencover.reportsPaths="/output-coverage/coverage.xml" \
                            /d:sonar.coverage.exclusions="src/Oragon.Common.RingBuffer.Tests/**/*,Examples/**/*,**/*.CodeGen.cs" \
                                /d:sonar.test.exclusions="src/Oragon.Common.RingBuffer.Testsx/**/*,Examples/**/*,**/*.CodeGen.cs" \
                                     /d:sonar.exclusions="src/Oragon.Common.RingBuffer.Tests/**/*,Examples/**/*,**/*.CodeGen.cs"
                        
                        dotnet build ./Oragon.Common.RingBuffer.sln
                        dotnet sonarscanner end /d:sonar.login="$SONARQUBE_KEY"
                        '''

                }
                
            }

        }

        stage('Pack') {

            agent {
                dockerfile {
                    // alwaysPull false
                    // image 'microsoft/dotnet:2.1-sdk'
                    // reuseNode false
                    args '-u root:root'
                }
            }

            when { buildingTag() }

            steps {

                script{

                    def projetcs = [
                        './src/Oragon.Common.RingBuffer/Oragon.Common.RingBuffer.csproj',
                    ]

                    if (env.BRANCH_NAME.endsWith("-alpha")) {

                        for (int i = 0; i < projetcs.size(); ++i) {
                            sh "dotnet pack ${projetcs[i]} --configuration Debug /p:PackageVersion=${BRANCH_NAME} --include-source --include-symbols --output ../output-packages"
                        }

                    } else if (env.BRANCH_NAME.endsWith("-beta")) {

                        for (int i = 0; i < projetcs.size(); ++i) {
                            sh "dotnet pack ${projetcs[i]} --configuration Release /p:PackageVersion=${BRANCH_NAME} --output ../output-packages"                        
                        }

                    } else {

                        for (int i = 0; i < projetcs.size(); ++i) {
                            sh "dotnet pack ${projetcs[i]} --configuration Release /p:PackageVersion=${BRANCH_NAME} --output ../output-packages"                        
                        }

                    }

                }

            }

        }

        stage('Publish') {

            agent {
                dockerfile {
                    // alwaysPull false
                    // image 'microsoft/dotnet:2.1-sdk'
                    // reuseNode false
                    args '-u root:root'
                }
            }

            when { buildingTag() }

            steps {
                
                script {
                    
                    def publishOnNuGet = ( env.BRANCH_NAME.endsWith("-alpha") == false );
                        
                        withCredentials([usernamePassword(credentialsId: 'myget-oragon', passwordVariable: 'MYGET_KEY', usernameVariable: 'DUMMY' )]) {

                        sh 'for pkg in ./output-packages/*.nupkg ; do dotnet nuget push "$pkg" -k "$MYGET_KEY" -s https://www.myget.org/F/oragon/api/v3/index.json -ss https://www.myget.org/F/oragon/symbols/api/v2/package ; done'
						
                        }

                    if (publishOnNuGet) {

                        withCredentials([usernamePassword(credentialsId: 'nuget-luizcarlosfaria', passwordVariable: 'NUGET_KEY', usernameVariable: 'DUMMY')]) {

                            sh 'for pkg in ./output-packages/*.nupkg ; do dotnet nuget push "$pkg" -k "$NUGET_KEY" -s https://api.nuget.org/v3/index.json ; done'

                        }

                    }                    
				}
            }
        }
    }
    post {

        always {
            node('master'){
                
                sh  '''
                
                #docker-compose down -v

                '''


            }
        }
    }
}